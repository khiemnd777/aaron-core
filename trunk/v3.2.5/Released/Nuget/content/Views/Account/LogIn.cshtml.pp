@model $rootnamespace$.Models.Accounts.LoginModel
@{
    ViewBag.Title = "Đăng nhập";
}

@using (Html.BeginForm()) {
    @Html.ValidationSummary(true)
    <div class="login">
        <div class="editor-label">
            @Html.LabelFor(model => model.Email):
        </div>
        <div class="editor-field">
            @Html.EditorFor(model => model.Email)
            @Html.ValidationMessageFor(model => model.Email)
        </div>

        <div class="editor-label">
            @Html.LabelFor(model => model.Password):
        </div>
        <div class="editor-field">
            @Html.EditorFor(model => model.Password)
            @Html.ValidationMessageFor(model => model.Password)
        </div>

        <div class="editor-label">
            
        </div>
        <div class="editor-field">
            @Html.EditorFor(model => model.RememberMe)
            @Html.ValidationMessageFor(model => model.RememberMe)
            @Html.LabelFor(model => model.RememberMe)
            -
            @Html.ActionLink("Quên mật khẩu?", "PasswordRecovery", "Account")
        </div>
        <input type="submit" value="Đăng nhập" />
    </div>
}
