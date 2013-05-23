@model List<ExternalAuthenticationMethodModel>
@using $rootnamespace$.Models.Accounts;
@foreach (var eam in Model)
{
    @Html.Action(eam.ActionName, eam.ControllerName, eam.RouteValues)
}