using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BazaR.Backend.Application.Abstractions.Files;

public sealed record UploadFile(
    Stream Content,
    string FileName,
    string ContentType,
    long SizeBytes);
