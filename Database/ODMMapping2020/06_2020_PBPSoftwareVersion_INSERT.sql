
INSERT [PBP].[PBPSoftwareVersion] ([PBPSoftwareVersionName], [TestQaVesrion], [IsLicenseVersion], [AddedDate], [AddedBy], [UpdatedDate], [UpdatedBy], [IsActive]) VALUES (N'2020.B1', N'1.009', 1, CAST(GETDATE() AS DateTime), N'superuser', NULL, NULL, 1)
GO
INSERT [PBP].[PBPSoftwareVersion] ([PBPSoftwareVersionName], [TestQaVesrion], [IsLicenseVersion], [AddedDate], [AddedBy], [UpdatedDate], [UpdatedBy], [IsActive]) VALUES (N'2020.B1', N'1.009', 0, CAST(GETDATE() AS DateTime), N'superuser', NULL, NULL, 1)
GO
