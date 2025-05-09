use pubs
go

create proc proc_filterProducts(@processor varchar(20),@pcount int out)
as
begin
	set @pcount=(Select count(*) from products where
	try_cast(json_value(details,'$.spec.processor')as nvarchar(20))=@processor)
end

declare @cnt int

exec proc_filterProducts 'i7',@cnt out
print concat ('The number of computers is',@cnt)


create table people(
id int primary key,
name nvarchar(20),
age int)

create or alter proc proc_bulkInsert(@filepath nvarchar(500))
as
begin
	declare @insertQuery nvarchar(max)
	set @insertQuery='BULK_INSERT people from '''+@filepath+'''
	with (
	FIRSTROW=2,
	FILEDTERMINATOR='','',
	ROWTERMINATOR=''\n'')'
	exec sp_executesql @insertQuery
end

exec proc_bulkInsert ''
with cteAuthors
as
(select au_id,concat(au_fname,'',au_lname) author_name from authors)

select * from cteAuthors


