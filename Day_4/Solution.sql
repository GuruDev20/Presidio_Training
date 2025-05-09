use northwind
go

--1) List all orders with the customer name and the employee who handled the order.
SELECT o.OrderID,c.CompanyName AS CustomerName,e.FirstName + ' ' + e.LastName AS EmployeeName
FROM Orders o
JOIN Customers c ON o.CustomerID = c.CustomerID
JOIN Employees e ON o.EmployeeID = e.EmployeeID;

--2) Get a list of products along with their category and supplier name.
SELECT p.ProductName,c.CategoryName,s.CompanyName AS SupplierName
FROM Products p
JOIN Categories c ON p.CategoryID = c.CategoryID
JOIN Suppliers s ON p.SupplierID = s.SupplierID;

--3) Show all orders and the products included in each order with quantity and unit price.
SELECT 
    o.OrderID,
    p.ProductName,
    od.Quantity,
    od.UnitPrice
FROM Orders o
JOIN [Order Details] od ON o.OrderID = od.OrderID
JOIN Products p ON od.ProductID = p.ProductID;

--4) List employees who report to other employees (manager-subordinate relationship).
SELECT 
    e.EmployeeID,
    e.FirstName + ' ' + e.LastName AS EmployeeName,
    m.FirstName + ' ' + m.LastName AS ManagerName
FROM Employees e
JOIN Employees m ON e.ReportsTo = m.EmployeeID;

--5) Display each customer and their total order count.
SELECT 
    c.CustomerID,
    c.CompanyName,
    COUNT(o.OrderID) AS TotalOrders
FROM Customers c
LEFT JOIN Orders o ON c.CustomerID = o.CustomerID
GROUP BY c.CustomerID, c.CompanyName;

--6) Find the average unit price of products per category.
SELECT 
    c.CategoryName,
    AVG(p.UnitPrice) AS AvgUnitPrice
FROM Products p
JOIN Categories c ON p.CategoryID = c.CategoryID
GROUP BY c.CategoryName;

--7) List customers where the contact title starts with 'Owner'.
SELECT 
    CustomerID,
    CompanyName,
    ContactTitle
FROM Customers
WHERE ContactTitle LIKE 'Owner%';

--8) Show the top 5 most expensive products.
SELECT TOP 5 
    ProductName,
    UnitPrice
FROM Products
ORDER BY UnitPrice DESC;

--9) Return the total sales amount (quantity × unit price) per order.
SELECT 
    od.OrderID,
    SUM(od.Quantity * od.UnitPrice) AS TotalSales
FROM [Order Details] od
GROUP BY od.OrderID;

--10) Create a stored procedure that returns all orders for a given customer ID.
DROP PROCEDURE IF EXISTS GetOrdersByCustomer;
GO
CREATE or ALTER PROCEDURE GetOrdersByCustomer(@CustomerID NVARCHAR(5))
AS
BEGIN
    SELECT * FROM Orders WHERE CustomerID = @CustomerID;
END
go
exec GetOrdersByCustomer 'QUICK'

--11) Write a stored procedure that inserts a new product.
CREATE or ALTER PROCEDURE InsertProduct (@ProductName NVARCHAR(40),@SupplierID INT,@CategoryID INT,@UnitPrice MONEY,@UnitsInStock SMALLINT = 0)
AS
BEGIN
    INSERT INTO Products (ProductName, SupplierID, CategoryID, UnitPrice, UnitsInStock)
    VALUES (@ProductName, @SupplierID, @CategoryID, @UnitPrice, @UnitsInStock);
END;
Go
EXEC InsertProduct @ProductName = 'SAMPLE', @SupplierID = 1,@CategoryID = 1,@UnitPrice = 19.99, @UnitsInStock = 100;

--12) Create a stored procedure that returns total sales per employee.
CREATE PROCEDURE GetSalesPerEmployee
AS
BEGIN
    SELECT 
        e.EmployeeID,
        e.FirstName + ' ' + e.LastName AS EmployeeName,
        SUM(od.UnitPrice * od.Quantity) AS TotalSales
    FROM Employees e
    JOIN Orders o ON e.EmployeeID = o.EmployeeID
    JOIN [Order Details] od ON o.OrderID = od.OrderID
    GROUP BY e.EmployeeID, e.FirstName, e.LastName;
END;
GO
EXEC GetSalesPerEmployee;

--13) Use a CTE to rank products by unit price within each category.
WITH RankedProducts AS (
    SELECT 
        ProductID,
        ProductName,
        CategoryID,
        UnitPrice,
        RANK() OVER (PARTITION BY CategoryID ORDER BY UnitPrice DESC) AS PriceRank
    FROM Products
)
SELECT * FROM RankedProducts;

--14) Create a CTE to calculate total revenue per product and filter products with revenue > 10,000.
WITH ProductRevenue AS (
    SELECT 
        p.ProductID,
        p.ProductName,
        SUM(od.Quantity * od.UnitPrice) AS Revenue
    FROM Products p
    JOIN [Order Details] od ON p.ProductID = od.ProductID
    GROUP BY p.ProductID, p.ProductName
)
SELECT * FROM ProductRevenue
WHERE Revenue > 10000;

--15) Use a CTE with recursion to display employee hierarchy.
WITH EmployeeHierarchy AS (
    SELECT 
        EmployeeID,
        FirstName + ' ' + LastName AS EmployeeName,
        ReportsTo,
        0 AS Level
    FROM Employees
    WHERE ReportsTo IS NULL

    UNION ALL

    SELECT 
        e.EmployeeID,
        e.FirstName + ' ' + e.LastName,
        e.ReportsTo,
        eh.Level + 1
    FROM Employees e
    INNER JOIN EmployeeHierarchy eh ON e.ReportsTo = eh.EmployeeID
)
SELECT * FROM EmployeeHierarchy
ORDER BY Level, EmployeeName;
