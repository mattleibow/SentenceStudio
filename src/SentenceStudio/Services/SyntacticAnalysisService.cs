using System.Diagnostics;
using SentenceStudio.Models;
using SQLite;
using SentenceStudio.Common;
using Scriban;
using System.Text.Json;

namespace SentenceStudio.Services;

public class SyntacticAnalysisService
{
    private SQLiteAsyncConnection Database;

    private AiService _aiService;

    private VocabularyService _vocabularyService;
    private List<Term> _terms;

    public SyntacticAnalysisService(IServiceProvider service)
    {
        _aiService = service.GetRequiredService<AiService>();
        _vocabularyService = service.GetRequiredService<VocabularyService>();
    }

    public async Task<List<SyntacticSentence>> GetSentences(int vocabularyListID)
    {
        VocabularyList vocab = await _vocabularyService.GetListAsync(vocabularyListID);

        if (vocab is null || vocab.Terms is null)
            return null;

        var random = new Random();
        
        _terms = vocab.Terms.OrderBy(t => random.Next()).Take(10).ToList();
        
        var prompt = string.Empty;     
        using Stream templateStream = await FileSystem.OpenAppPackageFileAsync("GetSentences.scriban-txt");
        using (StreamReader reader = new StreamReader(templateStream))
        {
            var template = Template.Parse(reader.ReadToEnd());
            prompt = await template.RenderAsync(new { terms = _terms });
        }

        Debug.WriteLine(prompt);
        
        try
        {
            string response = await _aiService.SendPrompt(prompt, true);
            var reply = JsonSerializer.Deserialize(response, JsonContext.Default.SyntacticSentencesResponse);
            return reply.Sentences;
        }
        catch (Exception ex)
        {
            // Handle any exceptions that occur during the process
            Debug.WriteLine($"An error occurred GetChallenges: {ex.Message}");
            return null;
        }
    }

    // async Task Init()
    // {
    //     if (Database is not null)
    //         return;

    //     Database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);

    //     CreateTableResult result;
        
    //     try
    //     {
    //         result = await Database.CreateTableAsync<SceneImage>();
    //     }
    //     catch (Exception ex)
    //     {
    //         Debug.WriteLine($"{ex.Message}");
    //         await App.Current.MainPage.DisplayAlert("Error", ex.Message, "Fix it");
    //     }
    // }

    // public async Task<List<SceneImage>> ListAsync()
    // {
    //     await Init();
    //     return await Database.Table<SceneImage>().ToListAsync();
    // }

    // public async Task<SceneImage> GetAsync(string url)
    // {
    //     await Init();
    //     return await Database.Table<SceneImage>().Where(i => i.Url == url).FirstOrDefaultAsync();
    // }

    // public async Task<int> SaveAsync(SceneImage item)
    // {
    //     await Init();
    //     int result = -1;
    //     if (item.ID != 0)
    //     {
    //         try
    //         {
    //             result = await Database.UpdateAsync(item);
    //         }
    //         catch (Exception ex)
    //         {
    //             Debug.WriteLine($"{ex.Message}");
    //         }
    //     }
    //     else
    //     {
    //         try
    //         {
    //             result = await Database.InsertAsync(item);
    //         }
    //         catch (Exception ex)
    //         {
    //             await App.Current.MainPage.DisplayAlert("Error", ex.Message, "Fix it");
    //         }
    //     }

    //     return result;
    // }
    

    // public async Task<int> DeleteAsync(SceneImage item)
    // {
    //     await Init();
    //     return await Database.DeleteAsync(item);
    // }

    // public async Task<SceneImage> GetRandomAsync()
    // {
    //     await Init();
    //     var images = await Database.Table<SceneImage>().ToListAsync();
    //     if (images.Count > 0)
    //     {
    //         var random = new Random();
    //         var index = random.Next(0, images.Count);
    //         var image = images[index];
    //         return image;
    //     }else{
    //         return null;
    //     }
    // }
}
