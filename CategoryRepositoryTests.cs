using BazaR.Backend.Domain.Categories;
using BazaR.Backend.Infrastructure.Persistence;
using BazaR.Backend.UnitTests.Helpers;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BazaR.Backend.UnitTests.Infrastructure.Persistence.Repositories;

public class CategoryRepositoryTests : IDisposable
{
    private readonly TestDbContextFactory _factory;
    private readonly AppDbContext _dbContext;
    private readonly CategoryRepository _repository;

    public CategoryRepositoryTests()
    {
        _factory = new TestDbContextFactory();
        _dbContext = _factory.CreateContext();
        _repository = new CategoryRepository(_dbContext);
    }

    public void Dispose()
    {
        _dbContext.Dispose();
        _factory.Dispose();
    }

    // ======================
    // ПОМОЩНИКИ ДЛЯ СОЗДАНИЯ ТЕСТОВЫХ ДАННЫХ
    // ======================

    private async Task<Category> CreateTestCategory(
        string name,
        CategoryId? parentId = null,
        int sortOrder = 0)
    {
        var category = Category.Create(name, parentId, sortOrder).Value;
        _repository.Add(category);
        await _dbContext.SaveChangesAsync();
        return category;
    }

    private async Task<Category> CreateCategoryTree()
    {
        // Создаем тестовую иерархию:
        // Electronics (root)
        //   ├── Smartphones
        //   │     ├── iPhone
        //   │     └── Android
        //   └── Laptops
        //         └── Gaming

        var electronics = await CreateTestCategory("Electronics");
        var smartphones = await CreateTestCategory("Smartphones", electronics.Id, 1);
        var laptops = await CreateTestCategory("Laptops", electronics.Id, 2);
        var iphone = await CreateTestCategory("iPhone", smartphones.Id, 1);
        var android = await CreateTestCategory("Android", smartphones.Id, 2);
        var gaming = await CreateTestCategory("Gaming", laptops.Id, 1);

        return electronics;
    }

    // ======================
    // ТЕСТЫ GetByIdAsync
    // ======================

    [Fact]
    public async Task GetByIdAsync_WhenCategoryExists_ReturnsCategory()
    {
        // Arrange
        var expectedCategory = await CreateTestCategory("Test Category");

        // Act
        var result = await _repository.GetByIdAsync(expectedCategory.Id, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedCategory.Id, result.Id);
        Assert.Equal("Test Category", result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_WhenCategoryNotExists_ReturnsNull()
    {
        // Arrange
        var nonExistentId = CategoryId.New();

        // Act
        var result = await _repository.GetByIdAsync(nonExistentId, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    // ======================
    // ТЕСТЫ ExistsAsync
    // ======================

    [Fact]
    public async Task ExistsAsync_WhenCategoryExists_ReturnsTrue()
    {
        // Arrange
        var category = await CreateTestCategory("Existing Category");

        // Act
        var exists = await _repository.ExistsAsync(category.Id, CancellationToken.None);

        // Assert
        Assert.True(exists);
    }

    [Fact]
    public async Task ExistsAsync_WhenCategoryNotExists_ReturnsFalse()
    {
        // Arrange
        var nonExistentId = CategoryId.New();

        // Act
        var exists = await _repository.ExistsAsync(nonExistentId, CancellationToken.None);

        // Assert
        Assert.False(exists);
    }

    // ======================
    // ТЕСТЫ GetChildrenAsync
    // ======================

    [Fact]
    public async Task GetChildrenAsync_ForRootCategory_ReturnsChildren()
    {
        // Arrange
        var root = await CreateTestCategory("Root");
        var child1 = await CreateTestCategory("Child 1", root.Id, 1);
        var child2 = await CreateTestCategory("Child 2", root.Id, 2);
        var child3 = await CreateTestCategory("Child 3", root.Id, 3);

        // Act
        var children = await _repository.GetChildrenAsync(root.Id, CancellationToken.None);

        // Assert
        Assert.Equal(3, children.Count);
        Assert.Equal(new[] { "Child 1", "Child 2", "Child 3" },
            children.Select(c => c.Name).ToArray());
    }

    [Fact]
    public async Task GetChildrenAsync_ForNullParentId_ReturnsRootCategories()
    {
        // Arrange
        var root1 = await CreateTestCategory("Root 1", sortOrder: 2);
        var root2 = await CreateTestCategory("Root 2", sortOrder: 1);
        var child = await CreateTestCategory("Child", root1.Id); // Не должен попасть в результат

        // Act
        var rootCategories = await _repository.GetChildrenAsync(null, CancellationToken.None);

        // Assert
        Assert.Equal(2, rootCategories.Count);
        Assert.Equal(new[] { "Root 2", "Root 1" }, // Сортировка по SortOrder
            rootCategories.Select(c => c.Name).ToArray());
    }

    [Fact]
    public async Task GetChildrenAsync_WhenNoChildren_ReturnsEmptyList()
    {
        // Arrange
        var leafCategory = await CreateTestCategory("Leaf");

        // Act
        var children = await _repository.GetChildrenAsync(leafCategory.Id, CancellationToken.None);

        // Assert
        Assert.Empty(children);
    }

    [Fact]
    public async Task GetChildrenAsync_ReturnsSortedBySortOrderThenByName()
    {
        // Arrange
        var parent = await CreateTestCategory("Parent");

        // Создаем категории в произвольном порядке
        await CreateTestCategory("Beta", parent.Id, sortOrder: 2);
        await CreateTestCategory("Alpha", parent.Id, sortOrder: 2); // Такое же SortOrder, сортировка по Name
        await CreateTestCategory("First", parent.Id, sortOrder: 1);
        await CreateTestCategory("Gamma", parent.Id, sortOrder: 3);

        // Act
        var children = await _repository.GetChildrenAsync(parent.Id, CancellationToken.None);

        // Assert
        Assert.Equal(new[] { "First", "Alpha", "Beta", "Gamma" },
            children.Select(c => c.Name).ToArray());
    }

    // ======================
    // ТЕСТЫ IsDescendantAsync
    // ======================

    [Fact]
    public async Task IsDescendantAsync_SameId_ReturnsTrue()
    {
        // Arrange
        var category = await CreateTestCategory("Test");

        // Act
        var isDescendant = await _repository.IsDescendantAsync(
            category.Id,
            category.Id,
            CancellationToken.None);

        // Assert
        Assert.True(isDescendant);
    }

    [Fact]
    public async Task IsDescendantAsync_DirectChild_ReturnsTrue()
    {
        // Arrange
        var parent = await CreateTestCategory("Parent");
        var child = await CreateTestCategory("Child", parent.Id);

        // Act
        var isDescendant = await _repository.IsDescendantAsync(
            parent.Id,
            child.Id,
            CancellationToken.None);

        // Assert
        Assert.True(isDescendant);
    }

    [Fact]
    public async Task IsDescendantAsync_GrandChild_ReturnsTrue()
    {
        // Arrange
        var grandparent = await CreateTestCategory("Grandparent");
        var parent = await CreateTestCategory("Parent", grandparent.Id);
        var child = await CreateTestCategory("Child", parent.Id);

        // Act
        var isDescendant = await _repository.IsDescendantAsync(
            grandparent.Id,
            child.Id,
            CancellationToken.None);

        // Assert
        Assert.True(isDescendant);
    }

    [Fact]
    public async Task IsDescendantAsync_NotDescendant_ReturnsFalse()
    {
        // Arrange
        var category1 = await CreateTestCategory("Category 1");
        var category2 = await CreateTestCategory("Category 2");
        var childOfCategory2 = await CreateTestCategory("Child", category2.Id);

        // Act
        var isDescendant = await _repository.IsDescendantAsync(
            category1.Id,
            childOfCategory2.Id,
            CancellationToken.None);

        // Assert
        Assert.False(isDescendant);
    }

    [Fact]
    public async Task IsDescendantAsync_WhenAncestorNotFound_ReturnsFalse()
    {
        // Arrange
        var category = await CreateTestCategory("Category");
        var nonExistentId = CategoryId.New();

        // Act
        var isDescendant = await _repository.IsDescendantAsync(
            nonExistentId,
            category.Id,
            CancellationToken.None);

        // Assert
        Assert.False(isDescendant);
    }

    [Fact]
    public async Task IsDescendantAsync_WhenDescendantNotFound_ReturnsFalse()
    {
        // Arrange
        var category = await CreateTestCategory("Category");
        var nonExistentId = CategoryId.New();

        // Act
        var isDescendant = await _repository.IsDescendantAsync(
            category.Id,
            nonExistentId,
            CancellationToken.None);

        // Assert
        Assert.False(isDescendant);
    }

    [Fact]
    public async Task IsDescendantAsync_ComplexTree_WorksCorrectly()
    {
        // Arrange - создаем сложное дерево
        var root = await CreateCategoryTree();
        var smartphones = (await _dbContext.Categories
            .FirstAsync(c => c.Name == "Smartphones"));
        var iphone = (await _dbContext.Categories
            .FirstAsync(c => c.Name == "iPhone"));
        var gaming = (await _dbContext.Categories
            .FirstAsync(c => c.Name == "Gaming"));

        // Assert различные проверки в дереве
        // 1. iPhone является потомком root
        Assert.True(await _repository.IsDescendantAsync(root.Id, iphone.Id, CancellationToken.None));

        // 2. iPhone является потомком smartphones
        Assert.True(await _repository.IsDescendantAsync(smartphones.Id, iphone.Id, CancellationToken.None));

        // 3. Gaming является потомком root
        Assert.True(await _repository.IsDescendantAsync(root.Id, gaming.Id, CancellationToken.None));

        // 4. iPhone НЕ является потомком gaming
        Assert.False(await _repository.IsDescendantAsync(gaming.Id, iphone.Id, CancellationToken.None));

        // 5. Gaming НЕ является потомком smartphones
        Assert.False(await _repository.IsDescendantAsync(smartphones.Id, gaming.Id, CancellationToken.None));
    }

    // ======================
    // ТЕСТЫ CRUD ОПЕРАЦИЙ
    // ======================

    [Fact]
    public async Task Add_AddsCategoryToDbContext()
    {
        // Arrange
        var category = Category.Create("New Category").Value;

        // Act
        _repository.Add(category);
        await _dbContext.SaveChangesAsync();

        // Assert
        var savedCategory = await _dbContext.Categories.FindAsync(category.Id);
        Assert.NotNull(savedCategory);
        Assert.Equal("New Category", savedCategory.Name);
    }

    [Fact]
    public async Task Remove_RemovesCategoryFromDbContext()
    {
        // Arrange
        var category = await CreateTestCategory("To be removed");

        // Act
        _repository.Remove(category);
        await _dbContext.SaveChangesAsync();

        // Assert
        var deletedCategory = await _dbContext.Categories.FindAsync(category.Id);
        Assert.Null(deletedCategory);
    }

    [Fact]
    public async Task IntegrationTest_FullCategoryLifecycle()
    {
        // Arrange
        var category = Category.Create("Test Category").Value;

        // Act 1: Add
        _repository.Add(category);
        await _dbContext.SaveChangesAsync();

        // Assert 1: Exists
        var exists = await _repository.ExistsAsync(category.Id, CancellationToken.None);
        Assert.True(exists);

        // Act 2: Get
        var retrieved = await _repository.GetByIdAsync(category.Id, CancellationToken.None);
        Assert.NotNull(retrieved);
        Assert.Equal("Test Category", retrieved.Name);

        // Act 3: Remove
        _repository.Remove(retrieved);
        await _dbContext.SaveChangesAsync();

        // Assert 3: Not exists
        exists = await _repository.ExistsAsync(category.Id, CancellationToken.None);
        Assert.False(exists);
    }

    // ======================
    // ТЕСТЫ СОРТИРОВКИ И ПОРЯДКА
    // ======================

    [Fact]
    public async Task Categories_WithSameSortOrder_AreSortedByName()
    {
        // Arrange
        var parent = await CreateTestCategory("Parent");

        // Создаем категории в обратном алфавитном порядке
        await CreateTestCategory("Zebra", parent.Id, sortOrder: 1);
        await CreateTestCategory("Apple", parent.Id, sortOrder: 1);
        await CreateTestCategory("Banana", parent.Id, sortOrder: 1);

        // Act
        var children = await _repository.GetChildrenAsync(parent.Id, CancellationToken.None);

        // Assert
        Assert.Equal(new[] { "Apple", "Banana", "Zebra" },
            children.Select(c => c.Name).ToArray());
    }

    [Fact]
    public async Task Categories_WithDifferentSortOrder_AreSortedBySortOrder()
    {
        // Arrange
        var parent = await CreateTestCategory("Parent");

        // Создаем категории в произвольном порядке SortOrder
        await CreateTestCategory("Third", parent.Id, sortOrder: 30);
        await CreateTestCategory("First", parent.Id, sortOrder: 10);
        await CreateTestCategory("Second", parent.Id, sortOrder: 20);

        // Act
        var children = await _repository.GetChildrenAsync(parent.Id, CancellationToken.None);

        // Assert
        Assert.Equal(new[] { "First", "Second", "Third" },
            children.Select(c => c.Name).ToArray());
    }
}