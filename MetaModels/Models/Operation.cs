namespace MetaModels.Models;

public record Operation(string VariableName, OperationType Type, int? Value = null);