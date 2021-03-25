USE weight
GO
DROP TABLE how_much
CREATE TABLE how_much
(
ID_calc	              int IDENTITY(1,1) PRIMARY KEY,  
date_work             datetime2(7) NOT NULL , 
name_rec              nvarchar(max),  
ID_rec                int NOT NULL,     
part_name             nvarchar(max),  
ID_part               int ,   
amount                float NOT NULL,      
note                  nvarchar(max),  
time_work             time NOT NULL, 
)


USE weight
GO
DROP TABLE weighting
CREATE TABLE weighting
(
ID_weighting          int IDENTITY(1,1) PRIMARY KEY,  
ID_calc               int NOT NULL,  
date_work             datetime2(7) NOT NULL , 
name                  nvarchar(max),  
ID_name               int NOT NULL,     
amount                float NOT NULL,   
amount_real           float NOT NULL,   
time_weighting        time NOT NULL, 
)