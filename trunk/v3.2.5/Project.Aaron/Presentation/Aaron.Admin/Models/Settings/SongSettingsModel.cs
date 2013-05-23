using System.ComponentModel.DataAnnotations;

namespace Aaron.Admin.Models.Settings
{
    public class SongSettingsModel
    {
        [Display(Name="Ca khúc mặc định")]
        public int DefaultSongId { get; set; }
        [Display(Name = "Kích thước khung Media player")]
        public string PlayerDimension { get; set; }
        [Display(Name = "Tự động phát nhạc")]
        public bool Autoplay { get; set; }
        [Display(Name = "Đường dẫn file Power Point")]
        public string UploadPowerPointPath { get; set; }
        [Display(Name = "Đường dẫn file Sheet")]
        public string UploadSheetPath { get; set; }
        [Display(Name = "Đường dẫn lưu ca khúc đã Convert")]
        public string ConvertPath { get; set; }
        [Display(Name = "Cho phép download")]
        public bool AllowDownload { get; set; }
        [Display(Name = "Cho phép download Power point")]
        public bool AllowDownloadPowerPoint { get; set; }
        [Display(Name = "Cho phép download Sheet")]
        public bool AllowDownloadSheet { get; set; }
        [Display(Name = "Cho phép download Mp3")]
        public bool AllowDownloadMp3 { get; set; }
        [Display(Name = "Đăng nhập để bình luận")]
        public bool SignInToComment { get; set; }
        [Display(Name = "Cho phép bình luận")]
        public bool AllowComment { get; set; }
        [Display(Name = "Cho phép xem Playlist")]
        public bool AllowViewPlaylist { get; set; }
        [Display(Name = "Sử dụng Youtube Chromeless")]
        public bool UseYoutubeChromeless { get; set; }
        [Display(Name = "Ca khúc hiển thị trong một trang")]
        public int SongsInPage { get; set; }
        [Display(Name = "Ca khúc liên quan [trình bày: Tiles]")]
        public int AssociatedSongs_Tiles { get; set; }
        [Display(Name = "Ca khúc liên quan [trình bày: List]")]
        public int AssociatedSongs_List { get; set; }
        [Display(Name = "Ca khúc quan tâm")]
        public int InterestSongs { get; set; }
        [Display(Name = "Ca khúc nghe nhiều")]
        public int TopSongs { get; set; }
        [Display(Name = "Ca khúc mới nhất")]
        public int NewestSongs { get; set; }
    }
}