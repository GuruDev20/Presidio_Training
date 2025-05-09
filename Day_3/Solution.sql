use pubs
go

select title,pub_name from titles join publishers on titles.pub_id=publishers.pub_id

select * from publishers

select * from titles

select * from publishers where pub_id not in 
(select distinct pub_id from titles)

select title,pub_name from titles right outer join publishers on titles.pub_id=publishers.pub_id

select title,pub_name from titles left outer join publishers on titles.pub_id=publishers.pub_id

select * from authors

select * from titleauthor

select * from titles

select au_id, title FROM titleauthor JOIN titles ON titleauthor.title_id = titles.title_id;

select * from publishers


select pub_name,title,ord_date from publishers join titles on publishers.pub_id=titles.pub_id join sales on titles.title_id=sales.title_id

select pub_name Publishers_Name,MIN(ord_date) First_Sales_Date from publishers left outer join titles on publishers.pub_id=titles.pub_id left outer join sales on titles.title_id=sales.title_id Group by pub_name order by 2 desc

SELECT T.title AS Book_Name,CONCAT(S.stor_address,',', S.city, ',', S.state) AS Store_Address
FROM
    sales AS SL 
JOIN
    titles AS T ON SL.title_id = T.title_id
JOIN
    stores AS S ON SL.stor_id = S.stor_id
ORDER BY 1;


--Benefits of stored procedure
	--performance (compilation,generation)
	--imporving security
	--reducing complexity

--variable name preced with @ 

create procedure proc_firstProcedure
as
begin 
	print 'Hello world'
end
go
exec proc_firstProcedure

create table products(
	id int identity(1,1) constraint pk_productId primary key,
	name nvarchar(100) not null,
	details nvarchar(max)
)


create or alter proc proc_InsertProduct(@pname nvarchar(100),@pdetails nvarchar(max))
as
begin
	insert into products(name,details)values(@pname,@pdetails)
end
go
exec proc_InsertProduct 'Laptop','{"brand":"HP","spec":{"ram":"16GB","processor":"i7"}}'

Select * from products

select JSON_QUERY(details,'$.spec')Product_Specification from products

create proc proc_updateProduct(@pid int,@newvalue nvarchar(20))
as 
begin
	update products set details=JSON_MODIFY(details,'$.spec.ram',@newvalue) where id=@pid
end
go
exec proc_updateProduct 1,'24GB'

select * from products

select id,name,JSON_VALUE(details,'$.brand')Brand_Name from products

create table post(
id int primary key,
title nvarchar(100),
user_id int,
body nvarchar(max)
)

drop procedure if exists proc_bulkInsert;





create or alter proc proc_bulkInsert(@jsondata nvarchar(max))
as
begin
	insert into post(user_id,id,title,body)
	select userId,id,title,body from openJson(@jsondata)
	with (userId int,id int,title nvarchar(100),body nvarchar(max))
end
go

declare @jsondata nvarchar(max) = '
[
    {
        "userId": 1,
        "id": 101,
        "title": "First Post",
        "body": "This is the body of the first post."
    },
    {
        "userId": 2,
        "id": 102,
        "title": "Second Post",
        "body": "This is the body of the second post."
    }
]';
exec proc_bulkInsert  @jsondata;

select * from products

select * from products where
TRY_CAST(json_value(details,'$.spec.processor') as nvarchar(20))='i7'

create or alter proc proc_fetchPost(@userId int)
as
begin
	select * from post where user_id=@userId
end
go

exec proc_fetchPost 2