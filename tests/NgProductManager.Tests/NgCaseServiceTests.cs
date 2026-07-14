using Microsoft.Data.Sqlite;
using NgProductManager.Models;
using NgProductManager.Services;

namespace NgProductManager.Tests;

[TestClass]
public class NgCaseServiceTests
{
    private string _databasePath = string.Empty;

    [TestInitialize]
    public void Setup()
    {
        _databasePath = Path.Combine(Path.GetTempPath(), $"ng-manager-tests-{Guid.NewGuid():N}.db");
    }

    [TestCleanup]
    public void Cleanup()
    {
        try
        {
            SqliteConnection.ClearAllPools();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
        catch
        {
            // ignore cleanup issues and retry file deletion below
        }

        for (var attempt = 0; attempt < 5; attempt++)
        {
            try
            {
                if (File.Exists(_databasePath))
                {
                    File.Delete(_databasePath);
                }

                return;
            }
            catch (IOException)
            {
                if (attempt == 4)
                {
                    throw;
                }

                Thread.Sleep(100);
            }
        }
    }

    [TestMethod]
    public void CreateInitialCase_SavesCaseAndInitialHistory()
    {
        var service = new NgCaseService(_databasePath);
        var modelId = service.CreateProductModel("M001", "モデルA");
        var defectReasonId = service.CreateDefectReason("寸法不良");
        var actionTypeId = service.CreateActionType("再加工");

        var caseId = service.CreateCaseWithInitialInspection(new CreateCaseRequest
        {
            LotNumber = "LOT-001",
            ProductModelId = modelId,
            SerialNumber = "SN-001",
            RegisteredAt = new DateTime(2024, 1, 10, 9, 0, 0),
            Notes = "初回登録",
            InspectionDateTime = new DateTime(2024, 1, 10, 9, 5, 0),
            Result = InspectionResult.Ng,
            DefectReasonId = defectReasonId,
            DefectDetails = "長さ不足",
            ActionTypeId = actionTypeId,
            ActionDetails = "再加工",
            InspectorName = "山田"
        });

        var caseDetail = service.GetCase(caseId);
        Assert.IsNotNull(caseDetail);
        Assert.AreEqual(1, caseDetail!.InspectionHistories.Count);
        Assert.AreEqual(NgCaseStatus.InProgress, caseDetail.Status);
        Assert.AreEqual("LOT-001", caseDetail.LotNumber);
    }

    [TestMethod]
    public void CreateInitialCase_FailsAtomically_WhenValidationFails()
    {
        var service = new NgCaseService(_databasePath);
        var modelId = service.CreateProductModel("M002", "モデルB");
        var defectReasonId = service.CreateDefectReason("傷");
        var actionTypeId = service.CreateActionType("修正");

        Assert.ThrowsException<InvalidOperationException>(() => service.CreateCaseWithInitialInspection(new CreateCaseRequest
        {
            LotNumber = string.Empty,
            ProductModelId = modelId,
            RegisteredAt = new DateTime(2024, 1, 10),
            InspectionDateTime = new DateTime(2024, 1, 10, 10, 0, 0),
            Result = InspectionResult.Ng,
            DefectReasonId = defectReasonId,
            ActionTypeId = actionTypeId,
            InspectorName = "鈴木"
        }));

        Assert.AreEqual(0, service.SearchCases(new NgCaseSearchCriteria()).Count);
    }

    [TestMethod]
    public void CreateInitialCase_RejectsUnknownMastersWithHelpfulMessage()
    {
        var service = new NgCaseService(_databasePath);
        var ex = Assert.ThrowsException<InvalidOperationException>(() => service.CreateCaseWithInitialInspection(new CreateCaseRequest
        {
            LotNumber = "LOT-100",
            ProductModelId = 999,
            RegisteredAt = new DateTime(2024, 1, 10),
            InspectionDateTime = new DateTime(2024, 1, 10, 10, 0, 0),
            Result = InspectionResult.Ng,
            DefectReasonId = 999,
            ActionTypeId = 999,
            InspectorName = "鈴木"
        }));

        StringAssert.Contains(ex.Message, "型番マスター");
    }

    [TestMethod]
    public void AddReinspectionHistory_AppendsHistoryWithoutEditingPreviousOnes()
    {
        var service = new NgCaseService(_databasePath);
        var modelId = service.CreateProductModel("M003", "モデルC");
        var initialReasonId = service.CreateDefectReason("寸法不良");
        var actionTypeId = service.CreateActionType("再加工");

        var caseId = service.CreateCaseWithInitialInspection(new CreateCaseRequest
        {
            LotNumber = "LOT-010",
            ProductModelId = modelId,
            RegisteredAt = new DateTime(2024, 2, 1),
            InspectionDateTime = new DateTime(2024, 2, 1, 8, 0, 0),
            Result = InspectionResult.Ng,
            DefectReasonId = initialReasonId,
            DefectDetails = "初回NG",
            ActionTypeId = actionTypeId,
            ActionDetails = "再加工",
            InspectorName = "加藤"
        });

        service.AddInspectionHistory(caseId, new InspectionHistoryInput
        {
            InspectionDateTime = new DateTime(2024, 2, 2, 9, 0, 0),
            Result = InspectionResult.Ng,
            DefectReasonId = initialReasonId,
            DefectDetails = "再度寸法不良",
            ActionTypeId = actionTypeId,
            ActionDetails = "再修正",
            InspectorName = "加藤",
            NextStatus = NgCaseStatus.PendingReinspection
        });

        var caseDetail = service.GetCase(caseId);
        Assert.AreEqual(2, caseDetail!.InspectionHistories.Count);
        Assert.AreEqual(NgCaseStatus.PendingReinspection, caseDetail.Status);
    }

    [TestMethod]
    public void AddInspectionHistory_AllowsMultipleHistoriesPerCase()
    {
        var service = new NgCaseService(_databasePath);
        var modelId = service.CreateProductModel("M004", "モデルD");
        var defectReasonId = service.CreateDefectReason("変形");
        var actionTypeId = service.CreateActionType("修正");
        var caseId = service.CreateCaseWithInitialInspection(new CreateCaseRequest
        {
            LotNumber = "LOT-011",
            ProductModelId = modelId,
            RegisteredAt = new DateTime(2024, 2, 3),
            InspectionDateTime = new DateTime(2024, 2, 3, 11, 0, 0),
            Result = InspectionResult.Ng,
            DefectReasonId = defectReasonId,
            DefectDetails = "初回",
            ActionTypeId = actionTypeId,
            ActionDetails = "修正",
            InspectorName = "伊藤"
        });

        service.AddInspectionHistory(caseId, new InspectionHistoryInput
        {
            InspectionDateTime = new DateTime(2024, 2, 4, 10, 0, 0),
            Result = InspectionResult.Ng,
            DefectReasonId = defectReasonId,
            DefectDetails = "追加NG",
            ActionTypeId = actionTypeId,
            ActionDetails = "修正",
            InspectorName = "伊藤",
            NextStatus = NgCaseStatus.InProgress
        });
        service.AddInspectionHistory(caseId, new InspectionHistoryInput
        {
            InspectionDateTime = new DateTime(2024, 2, 5, 10, 0, 0),
            Result = InspectionResult.Ok,
            InspectorName = "伊藤",
            NextStatus = NgCaseStatus.Closed
        });

        var caseDetail = service.GetCase(caseId);
        Assert.AreEqual(3, caseDetail!.InspectionHistories.Count);
        Assert.AreEqual(NgCaseStatus.Closed, caseDetail.Status);
    }

    [TestMethod]
    public void CloseCase_WithOkInspection_ClosesCaseAndSetsClosedAt()
    {
        var service = new NgCaseService(_databasePath);
        var modelId = service.CreateProductModel("M005", "モデルE");
        var defectReasonId = service.CreateDefectReason("外観不良");
        var actionTypeId = service.CreateActionType("部品交換");
        var caseId = service.CreateCaseWithInitialInspection(new CreateCaseRequest
        {
            LotNumber = "LOT-021",
            ProductModelId = modelId,
            RegisteredAt = new DateTime(2024, 3, 1),
            InspectionDateTime = new DateTime(2024, 3, 1, 8, 0, 0),
            Result = InspectionResult.Ng,
            DefectReasonId = defectReasonId,
            DefectDetails = "外観",
            ActionTypeId = actionTypeId,
            ActionDetails = "部品交換",
            InspectorName = "佐藤"
        });

        service.AddInspectionHistory(caseId, new InspectionHistoryInput
        {
            InspectionDateTime = new DateTime(2024, 3, 2, 8, 0, 0),
            Result = InspectionResult.Ok,
            InspectorName = "佐藤",
            NextStatus = NgCaseStatus.Closed
        });

        var caseDetail = service.GetCase(caseId);
        Assert.AreEqual(NgCaseStatus.Closed, caseDetail!.Status);
        Assert.IsTrue(caseDetail.ClosedAt.HasValue);
    }

    [TestMethod]
    public void GetCase_RetainsHistory_AfterCaseIsClosed()
    {
        var service = new NgCaseService(_databasePath);
        var modelId = service.CreateProductModel("M006", "モデルF");
        var defectReasonId = service.CreateDefectReason("動作不良");
        var actionTypeId = service.CreateActionType("再検査");
        var caseId = service.CreateCaseWithInitialInspection(new CreateCaseRequest
        {
            LotNumber = "LOT-031",
            ProductModelId = modelId,
            RegisteredAt = new DateTime(2024, 4, 1),
            InspectionDateTime = new DateTime(2024, 4, 1, 8, 0, 0),
            Result = InspectionResult.Ng,
            DefectReasonId = defectReasonId,
            DefectDetails = "初回",
            ActionTypeId = actionTypeId,
            ActionDetails = "再検査",
            InspectorName = "高橋"
        });

        service.AddInspectionHistory(caseId, new InspectionHistoryInput
        {
            InspectionDateTime = new DateTime(2024, 4, 2, 8, 0, 0),
            Result = InspectionResult.Ok,
            InspectorName = "高橋",
            NextStatus = NgCaseStatus.Closed
        });

        var caseDetail = service.GetCase(caseId);
        Assert.AreEqual(2, caseDetail!.InspectionHistories.Count);
        Assert.AreEqual(NgCaseStatus.Closed, caseDetail.Status);
    }

    [TestMethod]
    public void SearchCases_ExcludesClosedByDefault()
    {
        var service = new NgCaseService(_databasePath);
        var modelId = service.CreateProductModel("M007", "モデルG");
        var defectReasonId = service.CreateDefectReason("その他");
        var actionTypeId = service.CreateActionType("保留");

        var openCaseId = service.CreateCaseWithInitialInspection(new CreateCaseRequest
        {
            LotNumber = "LOT-041",
            ProductModelId = modelId,
            RegisteredAt = new DateTime(2024, 5, 1),
            InspectionDateTime = new DateTime(2024, 5, 1, 8, 0, 0),
            Result = InspectionResult.Ng,
            DefectReasonId = defectReasonId,
            ActionTypeId = actionTypeId,
            InspectorName = "松本"
        });

        var closedCaseId = service.CreateCaseWithInitialInspection(new CreateCaseRequest
        {
            LotNumber = "LOT-042",
            ProductModelId = modelId,
            RegisteredAt = new DateTime(2024, 5, 2),
            InspectionDateTime = new DateTime(2024, 5, 2, 8, 0, 0),
            Result = InspectionResult.Ng,
            DefectReasonId = defectReasonId,
            ActionTypeId = actionTypeId,
            InspectorName = "松本"
        });

        service.AddInspectionHistory(closedCaseId, new InspectionHistoryInput
        {
            InspectionDateTime = new DateTime(2024, 5, 3, 8, 0, 0),
            Result = InspectionResult.Ok,
            InspectorName = "松本",
            NextStatus = NgCaseStatus.Closed
        });

        var results = service.SearchCases(new NgCaseSearchCriteria());
        Assert.AreEqual(1, results.Count);
        Assert.AreEqual(openCaseId, results[0].Id);
    }

    [TestMethod]
    public void SearchCases_IncludesClosed_WhenRequested()
    {
        var service = new NgCaseService(_databasePath);
        var modelId = service.CreateProductModel("M008", "モデルH");
        var defectReasonId = service.CreateDefectReason("その他");
        var actionTypeId = service.CreateActionType("保留");

        var closedCaseId = service.CreateCaseWithInitialInspection(new CreateCaseRequest
        {
            LotNumber = "LOT-051",
            ProductModelId = modelId,
            RegisteredAt = new DateTime(2024, 6, 1),
            InspectionDateTime = new DateTime(2024, 6, 1, 8, 0, 0),
            Result = InspectionResult.Ng,
            DefectReasonId = defectReasonId,
            ActionTypeId = actionTypeId,
            InspectorName = "岡田"
        });

        service.AddInspectionHistory(closedCaseId, new InspectionHistoryInput
        {
            InspectionDateTime = new DateTime(2024, 6, 2, 8, 0, 0),
            Result = InspectionResult.Ok,
            InspectorName = "岡田",
            NextStatus = NgCaseStatus.Closed
        });

        var results = service.SearchCases(new NgCaseSearchCriteria(), includeClosed: true);
        Assert.AreEqual(1, results.Count);
        Assert.AreEqual(closedCaseId, results[0].Id);
    }

    [TestMethod]
    public void CloseCase_ClosesOpenCaseAndSetsClosedAt()
    {
        var service = new NgCaseService(_databasePath);
        var modelId = service.CreateProductModel("M011", "モデルK");
        var defectReasonId = service.CreateDefectReason("外観不良");
        var actionTypeId = service.CreateActionType("部品交換");

        var caseId = service.CreateCaseWithInitialInspection(new CreateCaseRequest
        {
            LotNumber = "LOT-061",
            ProductModelId = modelId,
            RegisteredAt = new DateTime(2024, 7, 1),
            InspectionDateTime = new DateTime(2024, 7, 1, 8, 0, 0),
            Result = InspectionResult.Ng,
            DefectReasonId = defectReasonId,
            ActionTypeId = actionTypeId,
            InspectorName = "森田"
        });

        service.CloseCase(caseId);

        var detail = service.GetCase(caseId);
        Assert.AreEqual(NgCaseStatus.Closed, detail!.Status);
        Assert.IsTrue(detail.ClosedAt.HasValue);
        Assert.IsTrue(detail.UpdatedAt >= detail.ClosedAt.Value);
    }

    [TestMethod]
    public void CloseCase_ThrowsForAlreadyClosedCase()
    {
        var service = new NgCaseService(_databasePath);
        var modelId = service.CreateProductModel("M012", "モデルL");
        var defectReasonId = service.CreateDefectReason("動作不良");
        var actionTypeId = service.CreateActionType("再検査");

        var caseId = service.CreateCaseWithInitialInspection(new CreateCaseRequest
        {
            LotNumber = "LOT-062",
            ProductModelId = modelId,
            RegisteredAt = new DateTime(2024, 7, 2),
            InspectionDateTime = new DateTime(2024, 7, 2, 8, 0, 0),
            Result = InspectionResult.Ng,
            DefectReasonId = defectReasonId,
            ActionTypeId = actionTypeId,
            InspectorName = "佐々木"
        });

        service.CloseCase(caseId);

        Assert.ThrowsException<InvalidOperationException>(() => service.CloseCase(caseId));
    }

    [TestMethod]
    public void SearchCases_FiltersByProductModelName()
    {
        var service = new NgCaseService(_databasePath);
        var matchingModelId = service.CreateProductModel("M013", "モデルM");
        var otherModelId = service.CreateProductModel("M014", "別モデル");
        var defectReasonId = service.CreateDefectReason("その他");
        var actionTypeId = service.CreateActionType("保留");

        service.CreateCaseWithInitialInspection(new CreateCaseRequest
        {
            LotNumber = "LOT-071",
            ProductModelId = matchingModelId,
            RegisteredAt = new DateTime(2024, 8, 1),
            InspectionDateTime = new DateTime(2024, 8, 1, 8, 0, 0),
            Result = InspectionResult.Ng,
            DefectReasonId = defectReasonId,
            ActionTypeId = actionTypeId,
            InspectorName = "中村"
        });

        service.CreateCaseWithInitialInspection(new CreateCaseRequest
        {
            LotNumber = "LOT-072",
            ProductModelId = otherModelId,
            RegisteredAt = new DateTime(2024, 8, 2),
            InspectionDateTime = new DateTime(2024, 8, 2, 8, 0, 0),
            Result = InspectionResult.Ng,
            DefectReasonId = defectReasonId,
            ActionTypeId = actionTypeId,
            InspectorName = "中村"
        });

        var results = service.SearchCases(new NgCaseSearchCriteria { ProductModel = "モデル" });
        Assert.AreEqual(2, results.Count);
        CollectionAssert.AreEquivalent(new[] { "LOT-071", "LOT-072" }, results.Select(x => x.LotNumber).ToArray());
    }

    [TestMethod]
    public void CreateProductModel_PreventsDuplicateCodes()
    {
        var service = new NgCaseService(_databasePath);
        service.CreateProductModel("M009", "モデルI");

        Assert.ThrowsException<InvalidOperationException>(() => service.CreateProductModel("M009", "別名"));
    }

    [TestMethod]
    public void GetActiveProductModels_ExcludesInactiveOnes()
    {
        var service = new NgCaseService(_databasePath);
        var id = service.CreateProductModel("M010", "モデルJ");
        service.SetProductModelActive(id, false);

        var activeModels = service.GetActiveProductModels();
        Assert.AreEqual(0, activeModels.Count);
    }
}
