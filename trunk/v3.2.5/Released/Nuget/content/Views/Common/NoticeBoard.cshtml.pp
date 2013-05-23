@model IEnumerable<NoticeModel>
@using $rootnamespace$.Models.Common;
@using Aaron.Core.Web;
@using Aaron.Core;
@if(!Model.IsNull() && Model.Count() > 0){
<div id="noticeBoard" class="noticeboard">
    @foreach (var n in Model)
    {
        <div class="noticeboard-content">
            @Html.SimplifyHtmlContent(n.Content)
        </div>
    }
    <input type="button" onclick="javascript: doNotDisturb();" value="Ẩn thông báo." />
</div>
<script type="text/javascript">
    $(document).ready(function () {
        doNotDisturb = function () {
            $.post('@Url.Action("CloseNoticeBoard", "Common")');
            $("#noticeBoard").fadeOut();
        }
    });
</script>
}