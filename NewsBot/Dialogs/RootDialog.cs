using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Collections.Generic;
using NewsBot.Models;
using System.Net.Http;
using Newtonsoft.Json;

namespace NewsBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        List<String> newsSources = new List<string> { "bbc-news", "cnn", "associated-press" };


        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            //var message = await result;
            var activity = await result as Activity;
            PromptDialog.Choice(
                context: context,
                resume: GetNewsAsync,
                options: newsSources,
                prompt: "Please select your preferred news source:",
                retry: "I support the aforementioned BBC, CNN and AP at this time. Please select one of those threee options"
                );
            //var activity = await result as Activity;
            //var myText = activity.Text.ToUpper();
            //var userName = activity.Text;
            //var askStart = activity.Text.ToUpper();
            //if (askStart.Equals("START")) {
            //}
            // calculate something for us to return
            //int length = (activity.Text ?? string.Empty).Length;
            // return our reply to the user
            //await context.PostAsync($"You sent {activity.Text} which was {length} characters");            
            //await context.PostAsync($"Hello {userName}, please select an option to get the news.");
            //context.Wait(MessageReceivedAsync);
        }


        //arko async method call gareko vayera this needs to change function type to async
        private async Task GetNewsAsync(IDialogContext context, IAwaitable<String> result)
        {
            string newsSource = await result;
            var newsArticles = await GetNews(newsSource.ToString());
            if (newsArticles.Status == "ok")
            {
                List<Attachment> list = new List<Attachment>();
                foreach (Article articleResponse in newsArticles.Articles)
                {
                    var articleCard = new HeroCard()
                    {
                        Title = articleResponse.Title,
                        Subtitle = articleResponse.Author + ',' + articleResponse.PublishedAt,
                        Text = articleResponse.Description,
                        Images = (articleResponse.UrlToImage != null ? new List<CardImage> { new CardImage(articleResponse.UrlToImage.AbsoluteUri) } : new List<CardImage> { new CardImage("http://saveabandonedbabies.org/wp-content/uploads/2015/08/default.png") }),
                        //Images = new List<CardImage> { new CardImage(articleResponse.UrlToImage.AbsoluteUri) },
                        //Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "View Full Story", value: articleResponse.Url.ToString()) }
                        Buttons = (articleResponse.Url != null ? new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "View Full Story", value: articleResponse.Url.ToString()) } : new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "View Full Story", value: articleResponse.Url.ToString()) })
                    };
                    list.Add(articleCard.ToAttachment());
                }
                var reply = context.MakeMessage();
                reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                reply.Attachments = list;
                await context.PostAsync(reply);
            }
            else
            {
                await context.PostAsync("An Error Occured Calling NewsAPI.");
            }
            PromptDialog.Choice(
                     context: context,
                     resume: GetNewsAsync,
                     options: newsSources,
                     prompt: "Do you want more News?,Please Select Your Preferred News Source:",
                     retry: "Currently I support only one of the News Source among:.");
        }

        //    string newsSource = await result;
        //    var newsArticles = await GetNews(newsSource.ToString());
        //    if (newsArticles.Status == "ok")
        //    {
        //        List<Attachment> list = new List<Attachment>();
        //        foreach (Article articleResponse in newsArticles.Articles)
        //        {
        //            var articleCard = new HeroCard()
        //            {
        //                Title = articleResponse.Title;
        //            Subtitle = articleResponse.Author;
        //            Text = articleResponse.Description;
        //        };
        //    }
        //}


        //List<Article> articles = liveResult.Result.Articles;
        //foreach (var article in articles)
        //{
        //       HeroCard heroCard = new HeroCard { Title = article.Title, Subtitle}
        //}
        //throw new NotImplementedException();
        //}

        private async Task<LiveResponse> GetNews(string newsSource)
        {
            LiveResponse liveResponse = new LiveResponse();
            try
            {
                var client = new HttpClient();
                var response = await client.GetAsync(string.Format("https://newsapi.org/v1/articles?source={0}&apiKey=7f29daf2862c4e3ba2956c754689df07", newsSource));
                var content = await response.Content.ReadAsStringAsync();
                liveResponse = JsonConvert.DeserializeObject<LiveResponse>(content);
                return liveResponse;
            }
            catch (Exception)
            {
                return liveResponse;
            }
        }

    }
}
 