SELECT
    a.Type,
    COUNT(*) AS TotalBookings,
    AVG(CAST(DATEDIFF(day, b.CheckIn, b.CheckOut) AS float)) AS AverageStayNights
FROM Apartments a
INNER JOIN Bookings b ON b.ApartmentId = a.Id
WHERE a.Type = @Type
GROUP BY a.Type
