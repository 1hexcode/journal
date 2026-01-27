using Journal.Repository;

namespace Journal.Data;

public class DatabaseSeeder
{
    private readonly MoodRepository _moodRepository;
    private readonly TagRepository _tagsRepository;
    private readonly UserRepository _userRepository;
    // Add other repositories as needed

    public DatabaseSeeder(MoodRepository moodRepository, TagRepository tagsRepository, UserRepository userRepository)
    {
        _moodRepository = moodRepository;
        _tagsRepository = tagsRepository;
        _userRepository = userRepository;
    }

    public async Task InitializeAsync()
    {
        await _moodRepository.InitializeAndSeedAsync();
        await _tagsRepository.InitializeAndSeedAsync();
        await _userRepository.InitializeAndSeedAsync();
    }
}