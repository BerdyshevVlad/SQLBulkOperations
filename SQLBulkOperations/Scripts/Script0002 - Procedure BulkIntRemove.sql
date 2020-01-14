CREATE PROCEDURE BulkIntRemove
(                       
		@TABLENAME nvarchar(255),
        @IDS IDLIST READONLY
		
)                                         
AS

SET NOCOUNT ON;

BEGIN TRY
        BEGIN TRANSACTION

        DECLARE @Results TABLE(id INTEGER)
		
		DELETE 
        FROM Books
        WHERE Id IN (SELECT ID FROM @IDS)

        

        COMMIT TRANSACTION
END TRY
BEGIN CATCH
        PRINT ERROR_MESSAGE();

        ROLLBACK TRANSACTION
        THROW; -- Rethrow exception
END CATCH
GO