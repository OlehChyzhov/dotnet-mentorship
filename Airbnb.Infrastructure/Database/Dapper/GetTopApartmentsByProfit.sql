SELECT TOP(@Count) 
    a.Title AS ApartmentTitle, 
    COUNT(*) AS NumberOfBookings, 
    SUM(BookedTotalPrice) AS TotalProfit
FROM Apartments a
LEFT JOIN Bookings b ON a.Id = b.ApartmentId
GROUP BY a.Id
ORDER BY TotalProfit DESC