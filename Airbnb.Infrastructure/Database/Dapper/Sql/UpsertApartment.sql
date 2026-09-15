MERGE INTO Apartments AS target
USING (SELECT @Id AS Id) AS source
ON target.Id = source.Id
WHEN MATCHED THEN
    UPDATE SET
        ExternalId = @ExternalId,
        Title = @Title,
        Description = @Description,
        Type = @Type,
        Country = @Country,
        City = @City,
        Address = @Address,
        MaxGuests = @MaxGuests,
        Bedrooms = @Bedrooms,
        Bathrooms = @Bathrooms,
        Kitchens = @Kitchens,
        LivingRooms = @LivingRooms,
        PricePerNight = @PricePerNight,
        IsListed = @IsListed,
        OwnerId = @OwnerId,
        CustomData = @CustomData
WHEN NOT MATCHED THEN
    INSERT (Id, ExternalId, Title, Description, Type, Country, City, Address,
            MaxGuests, Bedrooms, Bathrooms, Kitchens, LivingRooms, PricePerNight,
            IsListed, CreatedAt, OwnerId, CustomData)
    VALUES (@Id, @ExternalId, @Title, @Description, @Type, @Country, @City, @Address,
            @MaxGuests, @Bedrooms, @Bathrooms, @Kitchens, @LivingRooms, @PricePerNight,
            @IsListed, @CreatedAt, @OwnerId, @CustomData)
OUTPUT inserted.*;
