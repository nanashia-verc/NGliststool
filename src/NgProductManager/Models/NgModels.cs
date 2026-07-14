namespace NgProductManager.Models;

public enum InspectionResult
{
    Ng = 1,
    Ok = 2
}

public enum NgCaseStatus
{
    InProgress = 1,
    PendingReinspection = 2,
    Closed = 3
}

public enum MasterStatus
{
    Active = 1,
    Inactive = 0
}

public class NgCase
{
    public int Id { get; set; }
    public string LotNumber { get; set; } = string.Empty;
    public int ProductModelId { get; set; }
    public int? ProcessId { get; set; }
    public string? SerialNumber { get; set; }
    public int Status { get; set; }
    public string? Notes { get; set; }
    public DateTime RegisteredAt { get; set; }
    public DateTime? ClosedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class InspectionHistory
{
    public int Id { get; set; }
    public int NgCaseId { get; set; }
    public DateTime InspectionDateTime { get; set; }
    public int Result { get; set; }
    public int? DefectReasonId { get; set; }
    public string? DefectDetails { get; set; }
    public int? ActionTypeId { get; set; }
    public string? ActionDetails { get; set; }
    public string? InspectorName { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ProductModelMaster
{
    public int Id { get; set; }
    public string ModelCode { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public int IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CaseAttachment
{
    public int Id { get; set; }
    public int NgCaseId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string? ContentType { get; set; }
    public byte[] Content { get; set; } = Array.Empty<byte>();
    public DateTime CreatedAt { get; set; }
}

public class ProcessMaster
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int IsActive { get; set; }
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class DefectReasonMaster
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int IsActive { get; set; }
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class ActionTypeMaster
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int IsActive { get; set; }
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class NgCaseDetail
{
    public int Id { get; set; }
    public string LotNumber { get; set; } = string.Empty;
    public int ProductModelId { get; set; }
    public int? ProcessId { get; set; }
    public string ProductModelName { get; set; } = string.Empty;
    public string? ProcessName { get; set; }
    public string? SerialNumber { get; set; }
    public NgCaseStatus Status { get; set; }
    public string? Notes { get; set; }
    public DateTime RegisteredAt { get; set; }
    public DateTime? ClosedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<InspectionHistoryDetail> InspectionHistories { get; set; } = new();
}

public class InspectionHistoryDetail
{
    public int Id { get; set; }
    public DateTime InspectionDateTime { get; set; }
    public InspectionResult Result { get; set; }
    public string? DefectReasonName { get; set; }
    public string? DefectDetails { get; set; }
    public string? ActionTypeName { get; set; }
    public string? ActionDetails { get; set; }
    public string? InspectorName { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class NgCaseListItem
{
    public int Id { get; set; }
    public NgCaseStatus Status { get; set; }
    public string LotNumber { get; set; } = string.Empty;
    public string ProductModelName { get; set; } = string.Empty;
    public string? ProcessName { get; set; }
    public string? SerialNumber { get; set; }
    public DateTime RegisteredAt { get; set; }
    public DateTime? LatestInspectionDateTime { get; set; }
    public string? LatestDefectReasonName { get; set; }
    public string? LatestActionTypeName { get; set; }
    public int InspectionHistoryCount { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? ClosedAt { get; set; }
}

public class CreateCaseRequest
{
    public string LotNumber { get; set; } = string.Empty;
    public int ProductModelId { get; set; }
    public int? ProcessId { get; set; }
    public string? SerialNumber { get; set; }
    public DateTime RegisteredAt { get; set; }
    public string? Notes { get; set; }
    public DateTime InspectionDateTime { get; set; }
    public InspectionResult Result { get; set; }
    public int? DefectReasonId { get; set; }
    public string? DefectDetails { get; set; }
    public int? ActionTypeId { get; set; }
    public string? ActionDetails { get; set; }
    public string? InspectorName { get; set; }
}

public class InspectionHistoryInput
{
    public DateTime InspectionDateTime { get; set; }
    public InspectionResult Result { get; set; }
    public int? DefectReasonId { get; set; }
    public string? DefectDetails { get; set; }
    public int? ActionTypeId { get; set; }
    public string? ActionDetails { get; set; }
    public string? InspectorName { get; set; }
    public NgCaseStatus NextStatus { get; set; }
}

public class NgCaseSearchCriteria
{
    public string? FreeText { get; set; }
    public string? LotNumber { get; set; }
    public string? ProductModel { get; set; }
    public NgCaseStatus? Status { get; set; }
    public DateTime? RegisteredFrom { get; set; }
    public DateTime? RegisteredTo { get; set; }
}
