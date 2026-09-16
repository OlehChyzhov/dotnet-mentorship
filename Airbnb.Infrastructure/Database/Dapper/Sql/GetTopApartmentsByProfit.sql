SELECT TOP(@Count) 
    a.Title AS ApartmentTitle, 
    COUNT(*) AS NumberOfBookings, 
    SUM(BookedTotalPrice) AS TotalProfit
FROM Apartments a
LEFT JOIN Bookings b ON a.Id = b.ApartmentId
WHERE a.OwnerId = @UserId
GROUP BY a.Title
ORDER BY TotalProfit DESC