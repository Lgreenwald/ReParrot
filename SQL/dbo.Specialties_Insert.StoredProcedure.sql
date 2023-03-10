USE [ReParrot]
GO
/****** Object:  StoredProcedure [dbo].[Specialties_Insert]    Script Date: 3/7/2023 3:34:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:	Lauren Greenwald
-- Create date: 01/25/2023
-- Description:	Specialties Insert
-- Code Reviewer: Kathy Xayasomloth


-- MODIFIED BY: author
-- MODIFIED DATE:
-- Code Reviewer: 
-- Note: 
-- =============================================

CREATE PROC [dbo].[Specialties_Insert]
			@Name nvarchar(100)
			,@Description nvarchar(500)
			,@OrganizationId int
			,@CreatedBy int
			,@ModifiedBy int
			,@Id int OUTPUT

AS

/*-----------TEST CODE------------------------

	Declare @Id int = 0

	Declare @Name nvarchar(100) = 'Oil Change'
			,@Description nvarchar(500) = 'Basic Oil Change'
			,@OrganizationId int = 8
			,@CreatedBy int = 1
			,@ModifiedBy int = 1
		

	Execute dbo.Specialties_Insert
			@Name
			,@Description
			,@OrganizationId
			,@CreatedBy
			,@ModifiedBy
			,@Id

	Select *
	From dbo.Specialties
	Where Id=@Id
*/
BEGIN

		INSERT INTO [dbo].[Specialties]
					([Name]
					,[Description]
					,[OrganizationId]
					,[CreatedBy]
					,[ModifiedBy])
			VALUES
					(@Name
					,@Description
					,@OrganizationId
					,@CreatedBy
					,@ModifiedBy)

			SET @Id = SCOPE_IDENTITY();
END
GO
