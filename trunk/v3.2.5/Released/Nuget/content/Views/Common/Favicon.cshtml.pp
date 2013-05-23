@model $rootnamespace$.Models.Common.FaviconModel
@if (Model.Uploaded)
{
    <link rel="shortcut icon" href="@Model.FaviconUrl" />
}