using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using SocialClubNI.Models;
using SocialClubNI.Services;
using SocialClubNI.ViewModels;
using Blobr;

namespace SocialClubNI.Controllers
{
    [Authorize(Policy="IsLoggedIn")]
    public class AdminController : Controller
    {
        private readonly StorageWrapper storageWrapper;
        private readonly MixCloudProvider mixCloudProvider;
        private readonly PodcastFileProvider fileProvider;

        public AdminController(StorageWrapper storageWrapper, MixCloudProvider mixCloudProvider, PodcastFileProvider fileProvider)
        {
            this.storageWrapper = storageWrapper;
            this.mixCloudProvider = mixCloudProvider;
            this.fileProvider = fileProvider;
        }

        public IActionResult Index()
        {
            ViewBag.Title = "Admin";
            return View();
        }

        public IActionResult Upload()
        {
            ViewBag.Title = "Upload";
            return View();
        }

        public async Task<IActionResult> Delete(string season, string stub)
        {
            var podcasts = await storageWrapper.GetPageAsync<Podcast>($"podcasts-{season}");
            var episode = podcasts.Items.FirstOrDefault(p => p.Stub == stub);

            if(episode == null)
            {
                return RedirectToAction("Episodes");
            }

            var newPage = storageWrapper.CreatePage<Podcast>($"podcasts-{season}", podcasts.Items.Where(p => p.Stub != stub).ToList());
            await storageWrapper.SavePageAsync(newPage);
            return RedirectToAction("Episodes");
        }

        public async Task<IActionResult> Episodes()
        {
            ViewBag.Title = "Admin Episodes";
            // TODO add previous seasons
            var page = await storageWrapper.GetPageAsync<Podcast>($"podcasts-1617");
            var episodes = page.Items.OrderByDescending(p => p.Published).ToList();
            var vm = new ManageEpisodesViewModel() { Episodes = episodes };
            return View(vm);
        }
        
        public async Task<IActionResult> Create(string stub)
        {
            if(string.IsNullOrWhiteSpace(stub))
            {
                ViewBag.Title = "Create";
                return View("CreateStub");
            }

            var podcast = await mixCloudProvider.GetMixCloudMetata(stub);
            var viewModel = CreateEpisodeViewModel.FromPodcast(podcast);

            // TODO: read the current season value from settings
            viewModel.Season = "1617";
            viewModel.Mp3s = await BuildFilenameSelect();

            ViewBag.Title = "Create";
            return View("CreateEpisode", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateEpisodeViewModel episode)
        {
            if(ModelState.IsValid)
            {
                var podcasts = await storageWrapper.GetPageAsync<Podcast>($"podcasts-{episode.Season}");

                if(podcasts.Items.Any(p => p.Filename == episode.Filename))
                {
                    ModelState.AddModelError(string.Empty, $"The file '{episode.Filename}' already belongs to an existing podcast");
                }

                if(podcasts.Items.Any(p => p.Stub == episode.Stub))
                {
                    ModelState.AddModelError(string.Empty, $"Podcast with stub '{episode.Stub}' already exists.");
                }

                if(ModelState.ErrorCount > 0)
                {
                    episode.Mp3s = await BuildFilenameSelect();
                    return View("CreateEpisode", episode);
                }

                var podcast = new Podcast();
                podcast.Title = episode.Title;
                podcast.SubTitle = episode.SubTitle;
                podcast.Summary = episode.Summary;
                podcast.Published = episode.Published;
                podcast.Duration = episode.Duration;
                podcast.Filename = episode.Filename;
                podcast.Stub = episode.Stub; 
                podcast.Season = episode.Season;
                podcast.FileSize = await fileProvider.GetBlobSize(episode.Filename);

                podcasts.AddItem(podcast);
                await storageWrapper.SavePageAsync(podcasts);

                return RedirectToAction("Episode", "Home", new { season = episode.Season, stub = episode.Stub});
            }

            ViewBag.Title = "Create";
            return View(episode);
        }

        private async Task<ICollection<SelectListItem>> BuildFilenameSelect()
        {
            var filenames = await fileProvider.GetPodcastFilenames();
            return filenames.Select(f => new SelectListItem() { Text = f, Value = f}).ToList();
        }
    }
}