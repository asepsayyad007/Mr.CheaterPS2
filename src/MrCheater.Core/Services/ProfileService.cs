using CommunityToolkit.Mvvm.ComponentModel;
using MrCheater.Domain.Interfaces;
using MrCheater.Domain.Models;

namespace MrCheater.Core.Services;

public partial class ProfileService : ObservableObject
{
    private readonly IProfileRepository _profileRepository;
    private readonly ISettingsRepository _settingsRepository;
    private readonly AppLogger _logger;

    [ObservableProperty]
    private GameProfile? _activeProfile;

    [ObservableProperty]
    private List<GameProfile> _profiles = new();

    public event EventHandler<GameProfile?>? ActiveProfileChanged;

    public ProfileService(
        IProfileRepository profileRepository,
        ISettingsRepository settingsRepository,
        AppLogger logger)
    {
        _profileRepository = profileRepository;
        _settingsRepository = settingsRepository;
        _logger = logger;
    }

    public async Task InitializeAsync()
    {
        var list = await _profileRepository.GetAllProfilesAsync();
        Profiles = list;

        var settings = await _settingsRepository.LoadSettingsAsync();
        GameProfile? selected = null;

        if (!string.IsNullOrEmpty(settings.SelectedProfileId))
        {
            selected = list.FirstOrDefault(p => p.Id.Equals(settings.SelectedProfileId, StringComparison.OrdinalIgnoreCase));
        }

        selected ??= list.FirstOrDefault();
        SetActiveProfile(selected);
    }

    public void SetActiveProfile(GameProfile? profile)
    {
        if (ActiveProfile != profile)
        {
            ActiveProfile = profile;
            ActiveProfileChanged?.Invoke(this, profile);

            if (profile != null)
            {
                _logger.Info($"Active profile switched to: '{profile.Name}'");
                _ = Task.Run(async () =>
                {
                    var settings = await _settingsRepository.LoadSettingsAsync();
                    settings.SelectedProfileId = profile.Id;
                    await _settingsRepository.SaveSettingsAsync(settings);
                });
            }
        }
    }

    public async Task SaveProfileAsync(GameProfile profile)
    {
        await _profileRepository.SaveProfileAsync(profile);
        var list = await _profileRepository.GetAllProfilesAsync();
        Profiles = list;

        if (ActiveProfile?.Id == profile.Id)
        {
            ActiveProfile = profile;
        }

        _logger.Info($"Saved profile: '{profile.Name}'");
    }

    public async Task DeleteProfileAsync(string id)
    {
        await _profileRepository.DeleteProfileAsync(id);
        var list = await _profileRepository.GetAllProfilesAsync();
        Profiles = list;

        if (ActiveProfile?.Id == id)
        {
            SetActiveProfile(list.FirstOrDefault());
        }

        _logger.Info($"Deleted profile ID: {id}");
    }

    public async Task<GameProfile> DuplicateProfileAsync(GameProfile source)
    {
        var clone = source.Clone();
        await SaveProfileAsync(clone);
        return clone;
    }
}
