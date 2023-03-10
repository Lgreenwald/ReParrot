USE [ReParrot]
GO
/****** Object:  StoredProcedure [dbo].[Specialties_Delete_ById]    Script Date: 3/7/2023 3:34:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:	Lauren Greenwald
-- Create date: 01/25/2023
-- Description:	Specialties Delete By Id
-- Code Reviewer: Kathy Xayasomloth


-- MODIFIED BY: author
-- MODIFIED DATE:
-- Code Reviewer: 
-- Note: 
-- =============================================


CREATE proc [dbo].[Specialties_Delete_ById]

			@Id int

as
/*
Declare @Id int = 8

Execute dbo.Specialties_Select_ById @Id

Execute [dbo].[Specialties_Delete_ById] @Id

Execute dbo.Specialties_Select_ById @Id
*/

BEGIN

	DELETE From [dbo].[Specialties]
	WHERE Id = @Id

END
GO
