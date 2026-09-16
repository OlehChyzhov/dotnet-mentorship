SELECT
    a.Bedrooms AS Bedrooms,
    COUNT(*) AS ApartmentCount,
    AVG(a.PricePerNight) AS AveragePricePerNight
FROM Apartments a
WHERE a.Bedrooms = @NumOfBedrooms
GROUP BY a.Bedrooms
