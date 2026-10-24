# When to Mock

Mock at **system boundaries** only:

- External APIs (payment, email, etc.)
- Databases (sometimes - prefer test DB)
- Time/randomness
- File system (sometimes)

Don't mock:

- Your own classes/modules
- Internal collaborators
- Anything you control

## Designing for Mockability

At system boundaries, design interfaces that are easy to mock:

**1. Use dependency injection**

Pass external dependencies in rather than creating them internally:

```typescript
// Easy to mock
function processPayment(order, paymentClient) {
  return paymentClient.charge(order.total);
}

// Hard to mock
function processPayment(order) {
  const client = new StripeClient(process.env.STRIPE_KEY);
  return client.charge(order.total);
}
```

**2. Prefer SDK-style interfaces over generic fetchers**

Create specific functions for each external operation instead of one generic function with conditional logic:

```typescript
// GOOD: Each function is independently mockable
const api = {
  getUser: (id) => fetch(`/users/${id}`),
  getOrders: (userId) => fetch(`/users/${userId}/orders`),
  createOrder: (data) => fetch('/orders', { method: 'POST', body: data }),
};

// BAD: Mocking requires conditional logic inside the mock
const api = {
  fetch: (endpoint, options) => fetch(endpoint, options),
};
```

The SDK approach means:
- Each mock returns one specific shape
- No conditional logic in test setup
- Easier to see which endpoints a test exercises
- Type safety per endpoint

---

## Fakes vs. Mocks in Clean Architecture (.NET 10 / C#)

No contexto do backend YupAchei, adotamos a convenção de **Fakes leves em memória** em vez de bibliotecas dinâmicas de mocking (como Moq ou NSubstitute):

### Por que Fakes em Memória?
1. **Zero dependências extras**: Não polui o projeto de teste com frameworks adicionais.
2. **Resistência a refatoração**: Mocks exigem configuração manual de `.Returns(...)` e verificações rígidas de chamadas de métodos, quebrando testes quando a implementação interna muda sem alterar o comportamento público.
3. **Comportamento real de coleção**: Um fake simples com `List<T>` implementando `ILugarRepository` ou `IEventoRepository` se comporta exatamente como um repositório em memória previsível e determinístico.

```csharp
// Exemplo de Fake limpo implementando a interface do domínio
internal sealed class FakeLugarRepository : ILugarRepository
{
    private readonly List<Lugar> _itens;

    public FakeLugarRepository(IEnumerable<Lugar>? itensIniciais = null)
    {
        _itens = itensIniciais?.ToList() ?? [];
    }

    public Task<Lugar?> ObterPorIdAsync(Guid id, CancellationToken ct = default) =>
        Task.FromResult(_itens.FirstOrDefault(x => x.Id == id));

    public Task<Lugar?> ObterPorSlugAsync(string slug, CancellationToken ct = default) =>
        Task.FromResult(_itens.FirstOrDefault(x => string.Equals(x.Slogan, slug, StringComparison.OrdinalIgnoreCase)));

    public Task<IReadOnlyList<Lugar>> BuscarAsync(PlaceSearchCriteria cr, CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<Lugar>>(_itens.ToList());

    public Task<IReadOnlyList<Lugar>> ObterFallbackCuradoriaAsync(int limite = 5, CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<Lugar>>(_itens.Take(limite).ToList());

    public Task AdicionarAsync(Lugar lugar, CancellationToken ct = default)
    {
        _itens.Add(lugar);
        return Task.CompletedTask;
    }

    public Task AtualizarAsync(Lugar lugar, CancellationToken ct = default) => Task.CompletedTask;
}
```
