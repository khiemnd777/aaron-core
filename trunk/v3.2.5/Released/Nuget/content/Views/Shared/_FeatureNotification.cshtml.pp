@model FeatureNotificationModel
@using $rootnamespace$.Models.Common
@{
    /* If you would like to change the Layout.
    Layout = "~/Views/Shared/_ColumnsTwo.cshtml";
    */
    var type = (Model.Type == FeatureNotificationType.Upgrade) ? "đang cập nhật" : "đang xây dựng";
    Html.AddTitleParts(string.Format("{0} {1}", Model.Name, type));
}
<p>
    Tính năng @Model.Name @type, xin bạn vui lòng quay lại sau!
</p>
<br />    
@*<a href="@Model.BackUrl" class="t-button">Quay trở lại trang chủ</a>*@
@Html.ActionLink("Quay trở lại trang chủ", "BackHome", "Common", null, new { @class="t-button" })