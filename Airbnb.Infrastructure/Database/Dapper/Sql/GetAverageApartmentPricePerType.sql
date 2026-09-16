SELECT
    a.Type AS Type,
    COUNT(*) AS ApartmentCount,
    AVG(a.PricePerNight) AS AveragePricePerNight,
    MIN(a.PricePerNight) AS MinPricePerNight,
    MAX(a.PricePerNight) AS MaxPricePerNight
FROM Apartments a
WHERE a.Type = @Type
GROUP BY a.Type
