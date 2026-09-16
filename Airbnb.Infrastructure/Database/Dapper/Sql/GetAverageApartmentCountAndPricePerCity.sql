SELECT
    a.Country AS Country,
    a.City AS City,
    COUNT(*) AS ApartmentCount,
    AVG(a.PricePerNight) AS AveragePricePerNight
FROM Apartments a
WHERE a.City = @City
GROUP BY a.Country, a.City
