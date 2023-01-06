
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebClient;

public class CustomerClient : IDisposable
{
    private HttpClient mClient;
    private Uri mPath;

    public CustomerClient(System.Uri path)
    {
        mClient = new HttpClient();
        mPath = path;
    }

    public void Dispose()
    {
        ((IDisposable)mClient).Dispose();
    }

    public async Task<Customer?> GetAsync(long id)
    {
        Customer? customer = null;
        HttpResponseMessage response = await mClient.GetAsync(new System.Uri(mPath,id.ToString()));
        if (response.IsSuccessStatusCode)
        {
            customer = await response.Content.ReadAsAsync<Customer>();
        }
        return customer;
    }

    public async Task<long> PostAsync(CustomerCreateRequest request)
    {
        Customer customer = new Customer { Id = 0, Firstname = request.Firstname, Lastname = request.Lastname };
        HttpResponseMessage response = await mClient.PostAsJsonAsync(mPath,customer);
        response.EnsureSuccessStatusCode();
        // return URI of the created resource.
        return await response.Content.ReadAsAsync<long>();
    }



}