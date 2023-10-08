# ExpressWFA (BACKEND SIDE)

Simple fullstack authentication project using ASP.NET Core Web Api as backend and Windows Forms Application as frontend.

MANUAL
1- git clone :   https://github.com/fabiomarku1/ExpressAPI.git    (back)

2- tek SQL Server duhet krijuar nje user password:
2- At the SQL Server should be created a new user password ( also existing ones can be used) .

SSMS -> login as localhost -> Security  ->  Logins -> New Login ->  complete Login and Password -> Add "Server Roles" 
-> Connect with new Login and Password ->  re-enter new/old password ( initial auth ). 


3-Change ConnectionStrings at ExpressAPI.csproj  -> appsettings.json( appsettings.Developement.json) 
....
  "ConnectionStrings": {
    "sqlConnection": "Server=localhost;Database=SoftEx;User Id={loginName};Password={password};Trusted_Connection=False;MultipleActiveResultSets=True;TrustServerCertificate=True;"
  }, 
....

4- Open "Package Manager Console" :     PM>   Update-Database
Also Swagger documentation :   https://localhost:5001/swagger/index.html   


5-git clone :   https://github.com/fabiomarku1/ExpressWFA.git    (front)

6-login:
	admin   password123
	user    password123
	
	double-click record for EDIT 
	
7- Explore.....


@Fabio Marku
@marku_fabio@hotmail.com
 
