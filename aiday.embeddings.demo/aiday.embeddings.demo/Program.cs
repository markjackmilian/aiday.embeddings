// See https://aka.ms/new-console-template for more information

using aiday.embeddings.demo;
using aiday.embeddings.demo.Models;
using aiday.embeddings.demo.Services;
using aiday.embeddings.typesense;
using aiday.embeddings.typesense.Models;
using Microsoft.Extensions.DependencyInjection;
using mjm.nethelpers;

var services = new ServiceCollection();
ServiceConfiguration.ConfigureServices(services);

var serviceProvider = services.BuildServiceProvider();

Console.WriteLine("Press c for create embeddings, any other key to query");
var key = Console.ReadKey();

if (key.KeyChar == 'c')
    await App.CreateEmbeddings(serviceProvider);
else
    await App.QueryEmbeddings(serviceProvider);








