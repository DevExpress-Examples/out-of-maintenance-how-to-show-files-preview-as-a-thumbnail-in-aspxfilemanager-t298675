<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<%@ Register Assembly="DevExpress.Web.v20.1, Version=20.1.14.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <style type="text/css">
     
    </style>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <dx:ASPxFileManager ID="ASPxFileManager1" runat="server" Width="100%" Height="500px" ProviderType="Physical"
                 OnCustomThumbnail="ASPxFileManager1_CustomThumbnail" OnInit="ASPxFileManager1_Init">
                <Settings RootFolder="Files" ThumbnailFolder="~/Imgs" AllowedFileExtensions=".rtf,.doc,.docx,.txt,.xlsx,.html,.xls,.csv" />
                <SettingsEditing AllowCreate="True" AllowDelete="True" AllowMove="True" AllowRename="True" />
                <SettingsUpload Enabled="True">
                </SettingsUpload>
            </dx:ASPxFileManager>
        </div>
    </form>
</body>
</html>
