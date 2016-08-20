SELECT * FROM Adjustments WHERE ReqID = '06/01/2016_Negatives_1'
SELECT * FROM BomChanges WHERE RevisedItem = '497-0500770'
SELECT * FROM CarrierList
SELECT * FROM CCTracker WHERE CountID = 20163594 
SELECT * FROM CCTracker WHERE CountDate >= '8/6/16'
SELECT * FROM CountSchedule WHERE ActualDate = '7/20/16'
SELECT * FROM DynamicInbMetrics
SELECT * FROM Employees WHERE FirstName = 'Patrick'
SELECT * FROM InboundActivity
SELECT * FROM MetricsAdj ORDER BY Date
SELECT * FROM MetricsInbound
SELECT * FROM MetricsInv WHERE Date >= '4/1/16' AND Date <= '5/27/16'
SELECT * FROM Orders
SELECT * FROM ReplenSAAG
SELECT * FROM RcvSchedule
SELECT * FROM UnitCost WHERE Item = '497-0499800'
SELECT * FROM WorkBenchCounts
SELECT * FROM MaterialRequests
SELECT * FROM MaterialReference


ALTER TABLE InboundActivity ALTER COLUMN LPH float

ALTER TABLE DynamicInbMetrics ADD CategoryNumValue float null
Update InboundActivity Set Type = 'REC' Where ID = 7 AND Difference != 0
SELECT * FROM DynamicInbMetrics
--insert into CCTracker values('5/25/16', 'MID', 'Available', 'BB051004C','1634-0046-8801', 'PR10262:T88 V Thermal Gray Serial', 0, 120, 120, null, null,'Laterius', null, 47, 193.8119, 'SecondReady', '20162316')
--insert into UnitCost values('5976-K838-V001', '5976 Tall Pole Kit (Black)', 0)
DELETE FROM RcvSchedule where ID > 1020
SELECT * FROM MetricsInbounds
UPDATE CCTracker Set CountStatus = 'FirstSubmitReview' Where CountID = 20163601 and Difference != 0
