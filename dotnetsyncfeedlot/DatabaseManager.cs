using Couchbase.Lite;
using Couchbase.Lite.DI;
using Couchbase.Lite.Sync;

namespace dotnetsyncfeedlot;

public class DatabaseManager
{
    private string _databaseName = "feedlot-test";
    private Replicator? _replicator;
    private ListenerToken _replicatorListenerToken;
    private bool _replicationComplete = false;
    private readonly Uri _remoteSyncUrl = new Uri("ws://localhost:4984/feedlot-test");
    public Database GetDatabase()
    {
        var defaultDirectory = Service.GetInstance<IDefaultDirectoryResolver>().DefaultDirectory();
        var databaseConfig = new DatabaseConfiguration
        {
            Directory = defaultDirectory
        };
        return new Database(_databaseName, databaseConfig);
    }
    
    public void StartReplicationAsync()
        // end::startSync[]
    {
        var database = GetDatabase();
        var targetUrlEndpoint = new URLEndpoint(new Uri(_remoteSyncUrl, _databaseName));
        // tag::replicationconfig[]
        var configuration = new ReplicatorConfiguration(database, targetUrlEndpoint) // <1>
        {
            
            Authenticator = new BasicAuthenticator("alice", "password"),
            ReplicatorType = ReplicatorType.Pull,
            Channels = new [] {"ref-data"},
            Continuous = false, // <3>
        };
        // end::replicationconfig[]
        // tag::replicationinit[]
        _replicator = new Replicator(configuration);
        // end::replicationinit[]
        // tag::replicationlistener[]
        _replicatorListenerToken = _replicator.AddChangeListener(OnReplicatorUpdate);
        // end::replicationlistener[]
        // tag::replicationstart[]
        _replicator.Start();
        // end::replicationstart[]
    }
    private void OnReplicatorUpdate(object sender, ReplicatorStatusChangedEventArgs e)
    {
        var status = e.Status;
        switch (status.Activity)
        {
            case ReplicatorActivityLevel.Busy:
                break;
            case ReplicatorActivityLevel.Connecting:
                Console.WriteLine("Connecting to Sync Gateway.");
                break;
            case ReplicatorActivityLevel.Idle:
                break;
            case ReplicatorActivityLevel.Offline:
                break;
            case ReplicatorActivityLevel.Stopped:
                _replicationComplete = true;
                Console.WriteLine("Completed syncing documents.");
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    public bool GetStatus()
    {
        return _replicationComplete;
    }
}
