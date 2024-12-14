// See https://aka.ms/new-console-template for more information

using Kudo.Storage.Context;
using Kudo.Storage.Repositories;

var databaseConnectionString =
    "Server=localhost;Port=5432;Database=KUDO_DATABASE_TEST;Username=postgres;Password=postgres;Include Error Detail=true";

using (var databaseContext = DatabaseContext.Create(databaseConnectionString))
{
    await databaseContext.EnsureMigratedDatabaseAsync(CancellationToken.None);
}

using (var databaseContext = DatabaseContext.Create(databaseConnectionString))
{
    var storageRepository = StorageRepository.Create(databaseContext);

    var result = await storageRepository.CreateStorageDefaultDataAsync(CancellationToken.None);

    Console.WriteLine(result
        ? "\t... хранилище пустое, выполнено заполнение данными 'по умолчанию'..."
        : "\t... хранилище содержит данные, заполнения данными 'по умолчанию' не требуется...");
}