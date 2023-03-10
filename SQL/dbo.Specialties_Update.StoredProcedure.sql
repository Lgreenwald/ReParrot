USE [ReParrot]
GO
/****** Object:  StoredProcedure [dbo].[Specialties_Update]    Script Date: 3/7/2023 3:34:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:	Lauren Greenwald
-- Create date: 01/26/2023
-- Description:	Specialties Update
-- Code Reviewer: Kathy Xayasomloth


-- MODIFIED BY: author
-- MODIFIED DATE:
-- Code Reviewer: 
-- Note: 
-- =============================================

CREATE proc [dbo].[Specialties_Update]
			@Name nvarchar(100)
			,@Description nvarchar(500)
			,@OrganizationId int
			,@ModifiedBy int
			,@Id int 
AS
/*-------------------------TEST CODE-------------------------

	Declare @Id int = 1
			,@Name nvarchar(100) = 'Oil Change'
			,@Description nvarchar(500) = 'Standard Oil Change Service'
			,@OrganizationId int = 1
			,@ModifiedBy int = 1

	SELECT *
	FROM [dbo].[Specialties]
	WHERE Id = @Id

	EXECUTE [dbo].[Specialties_Update]
			@Name
			,@Description
			,@OrganizationId
			,@ModifiedBy
			,@Id

	SELECT *
	FROM [dbo].[Specialties]
	WHERE Id = @Id
*/

BEGIN

	Declare @DateNow datetime2(7) = getutcdate()

	UPDATE [dbo].[Specialties]
	 SET   [Name] = @Name
		  ,[Description] = @Description
		  ,[OrganizationId] = @OrganizationId
		  ,[ModifiedBy] = @ModifiedBy
		  ,[DateModified] = @DateNow

	WHERE Id = @Id

END
GO
