using System;
using osu.Framework.Development;
using osu.Framework.Platform;
using Realms;

namespace fluXis.Game.Database;

public class FluXisRealm : IDisposable
{
    private Realm updateRealm;

    public Realm Realm
    {
        get
        {
            if (!ThreadSafety.IsUpdateThread)
                throw new InvalidOperationException("Realm can only be accessed from the update thread.");

            return updateRealm ??= getInstance();
        }
    }

    private readonly Storage storage;

    private RealmConfiguration config => new(storage.GetFullPath("fluxis.realm"))
    {
        SchemaVersion = 1
    };

    public FluXisRealm(Storage storage)
    {
        this.storage = storage;
    }

    private Realm getInstance()
    {
        return Realm.GetInstance(config);
    }

    public T Run<T>(Func<Realm, T> action)
    {
        if (ThreadSafety.IsUpdateThread)
            return action(Realm);

        using (var realm = getInstance())
            return action(realm);
    }

    public void Run(Action<Realm> action)
    {
        if (ThreadSafety.IsUpdateThread)
            action(Realm);
        else
        {
            using (var realm = getInstance())
                action(realm);
        }
    }

    public T RunWrite<T>(Func<Realm, T> action)
    {
        if (ThreadSafety.IsUpdateThread)
            return write(Realm, action);

        using (var realm = getInstance())
            return write(realm, action);
    }

    public void RunWrite(Action<Realm> action)
    {
        if (ThreadSafety.IsUpdateThread)
            write(Realm, action);
        else
        {
            using (var realm = getInstance())
                write(realm, action);
        }
    }

    private T write<T>(Realm realm, Func<Realm, T> func)
    {
        Transaction transaction = null;

        try
        {
            if (!realm.IsInTransaction)
                transaction = realm.BeginWrite();

            var result = func(realm);
            transaction?.Commit();
            return result;
        }
        finally
        {
            transaction?.Dispose();
        }
    }

    private void write(Realm realm, Action<Realm> func)
    {
        Transaction transaction = null;

        try
        {
            if (!realm.IsInTransaction)
                transaction = realm.BeginWrite();

            func(realm);
            transaction?.Commit();
        }
        finally
        {
            transaction?.Dispose();
        }
    }

    public void Dispose()
    {
        updateRealm?.Dispose();
    }
}
