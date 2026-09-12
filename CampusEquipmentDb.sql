CREATE DATABASE CampusEquipmentDb;
GO

USE CampusEquipmentDb;
GO

CREATE TABLE Department
(
    DepartmentId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(255) NULL
);
GO

CREATE TABLE Equipment
(
    EquipmentId INT IDENTITY(1,1) PRIMARY KEY,

    AssetCode NVARCHAR(50) NOT NULL,

    Name NVARCHAR(100) NOT NULL,

    Category NVARCHAR(100) NOT NULL,

    Brand NVARCHAR(100) NULL,

    Model NVARCHAR(100) NULL,

    PurchaseDate DATE NULL,

    Status NVARCHAR(50) NOT NULL,

    DepartmentId INT NOT NULL,

    CONSTRAINT UQ_Equipment_AssetCode
        UNIQUE (AssetCode),

    CONSTRAINT FK_Equipment_Department
        FOREIGN KEY (DepartmentId)
        REFERENCES Department(DepartmentId)
);
GO

INSERT INTO Department
(
    Name,
    Description
)
VALUES
(
    'Information Technology',
    'Information Technology Department'
),
(
    'Computer Science',
    'Computer Science Department'
),
(
    'Engineering',
    'Engineering Department'
),
(
    'Business Administration',
    'Business Administration Department'
);
GO

INSERT INTO Equipment
(
    AssetCode,
    Name,
    Category,
    Brand,
    Model,
    PurchaseDate,
    Status,
    DepartmentId
)
VALUES
(
    'PC-001',
    'Desktop Computer',
    'Computer',
    'Dell',
    'OptiPlex 7010',
    '2025-01-15',
    'Available',
    1
),
(
    'PC-002',
    'Desktop Computer',
    'Computer',
    'HP',
    'ProDesk 400',
    '2024-06-10',
    'Assigned',
    2
),
(
    'PRJ-001',
    'Projector',
    'Projector',
    'Epson',
    'EB-X06',
    '2024-03-20',
    'Available',
    1
),
(
    'LAP-001',
    'Laptop',
    'Computer',
    'Lenovo',
    'ThinkPad E14',
    '2023-08-05',
    'UnderMaintenance',
    3
),
(
    'PC-003',
    'Desktop Computer',
    'Computer',
    'Acer',
    'Veriton',
    '2022-02-15',
    'Retired',
    2
);
GO

SELECT
    e.EquipmentId,
    e.AssetCode,
    e.Name,
    e.Category,
    e.Brand,
    e.Model,
    e.PurchaseDate,
    e.Status,
    d.Name AS Department
FROM Equipment e
INNER JOIN Department d
    ON e.DepartmentId = d.DepartmentId;
GO