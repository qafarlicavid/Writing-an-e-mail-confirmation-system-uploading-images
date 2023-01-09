namespace DemoApplication.Areas.Client.ViewModels.Home.Index
{
    public class SliderListViewModel
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string ButtonName { get; set; }
        public string ImageUrl { get; set; }
        public string ButtonRedirectUrl { get; set; }
        public int Order { get; set; }
        public SliderListViewModel(string title, string content, string buttonName, string imageUrl, string buttonRedirectUrl, int order)
        {
            Title = title;
            Content = content;
            ButtonName = buttonName;
            ImageUrl = imageUrl;
            ButtonRedirectUrl = buttonRedirectUrl;
            Order = order;
        }
    }
}
