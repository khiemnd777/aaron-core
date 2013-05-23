﻿@model TopicModel
@using $rootnamespace$.Models.Topics;
@if (Model.IsPasswordProtected)
{
    <script type="text/javascript">    
        $(document).ready(function () {
            $('#ph-topic-@Model.Id').hide();            
            $('#ph-password-@Model.Id #password-@Model.Id').select().focus();
        });    
        function OnAuthenticateSuccess@(Model.Id)(context) {
            if (context.Authenticated)
            {
                $('#ph-title-@Model.Id .topic-html-content-title h2.topic-html-content-header').html(context.Title);
                if ($('#ph-title-@Model.Id .topic-html-content-title h2.topic-html-content-header').text().length == 0)
                {
                    $('#ph-title-@Model.Id').hide();
                }
                $('#ph-topic-@Model.Id .topic-html-content-body').html(context.Body);
                $('#ph-password-@Model.Id').hide();
                $('#ph-topic-@Model.Id').show();
            }
            else
            {
                $('#password-error-@Model.Id').text(context.Error);
                $('#ph-password-@Model.Id #password-@Model.Id').select().focus();
            }
        }
    </script>
    <div id="ph-password-@Model.Id">
        @using (Ajax.BeginRouteForm("TopicAuthenticate", new AjaxOptions
        {
            HttpMethod = "Post",
            OnSuccess = "OnAuthenticateSuccess" + @Model.Id,
            LoadingElementId = "authenticate-progress-" + @Model.Id
        }))
        {
            @Html.HiddenFor(model => model.Id)
            <div class="enter-password-title">
                Nhập mật khẩu
            </div>
            <div class="enter-password-form">
                @Html.Password("password", null, new { id = "password-" + @Model.Id })
                <input type="submit" value="Chấp nhận" class="button-1 topic-password-button"/>
                <span id="authenticate-progress-@Model.Id" style="display: none;" class="please-wait">Xin vui lòng chờ...</span>
            </div>
            <div class="password-error">
                <span id="password-error-@Model.Id"></span>
            </div>
        }
    </div>
    <div class="topic-html-content" id="ph-topic-@Model.Id">
        <div id="ph-title-@Model.Id">
            <div class="topic-html-content-title">
                <h2 class="topic-html-content-header">
                    @Model.Title</h2>
            </div>
            <div class="clear">
            </div>
        </div>
        <div class="topic-html-content-body">
            @Html.Raw(Model.Body)
        </div>
    </div>
}
else
{
    <div class="topic-html-content">
        @if (!String.IsNullOrEmpty(Model.Title))
        {
            <div class="topic-html-content-title">
                <h2 class="topic-html-content-header">
                    @Model.Title</h2>
            </div>
            <div class="clear">
            </div>
        }
        <div class="topic-html-content-body">
            @Html.Raw(Model.Body)
        </div>
    </div>
}