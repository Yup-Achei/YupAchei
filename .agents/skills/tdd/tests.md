# Good and Bad Tests

## Good Tests

**Integration-style**: Test through real interfaces, not mocks of internal parts.

```typescript
// GOOD: Tests observable behavior (TypeScript)
test("user can checkout with valid cart", async () => {
  const cart = createCart();
  cart.add(product);
  const result = await checkout(cart, paymentMethod);
  expect(result.status).toBe("confirmed");
});
```

Characteristics:

- Tests behavior users/callers care about
- Uses public API only
- Survives internal refactors
- Describes WHAT, not HOW
- One logical assertion per test

## Bad Tests

**Implementation-detail tests**: Coupled to internal structure.

```typescript
// BAD: Tests implementation details
test("checkout calls paymentService.process", async () => {
  const mockPayment = jest.mock(paymentService);
  await checkout(cart, payment);
  expect(mockPayment.process).toHaveBeenCalledWith(cart.total);
});
```

Red flags:

- Mocking internal collaborators
- Testing private methods
- Asserting on call counts/order
- Test breaks when refactoring without behavior change
- Test name describes HOW not WHAT
- Verifying through external means instead of interface

```typescript
// BAD: Bypasses interface to verify
test("createUser saves to database", async () => {
  await createUser({ name: "Alice" });
  const row = await db.query("SELECT * FROM users WHERE name = ?", ["Alice"]);
  expect(row).toBeDefined();
});

// GOOD: Verifies through interface
test("createUser makes user retrievable", async () => {
  const user = await createUser({ name: "Alice" });
  const retrieved = await getUser(user.id);
  expect(retrieved.name).toBe("Alice");
});
```

**Tautological tests**: Expected value restates the implementation, so the test passes by construction.

```typescript
// BAD: Expected value is recomputed the way the code computes it
test("calculateTotal sums line items", () => {
  const items = [{ price: 10 }, { price: 5 }];
  const expected = items.reduce((sum, i) => sum + i.price, 0);
  expect(calculateTotal(items)).toBe(expected);
});

// GOOD: Expected value is an independent, known literal
test("calculateTotal sums line items", () => {
  expect(calculateTotal([{ price: 10 }, { price: 5 }])).toBe(15);
});
```

---

## C# / .NET 10 & xUnit Examples (Clean Architecture)

### 1. Application Service com Fake em Memória (Red → Green → Refactor)

No projeto `YupAchei.Application.Tests`, testamos o serviço de aplicação (`CatalogoService`) através de sua interface pública (`ICatalogoService`), utilizando uma implementação Fake leve em memória de `ILugarRepository`:

```csharp
// GOOD: Testa comportamento observável usando Fake em memória (sem bibliotecas de mock)
public class CatalogoServiceTests
{
    private sealed class FakeLugarRepository : ILugarRepository
    {
        private readonly List<Lugar> _lugares = [];

        public FakeLugarRepository(IEnumerable<Lugar>? seed = null)
        {
            if (seed is not null) _lugares.AddRange(seed);
        }

        public Task<Lugar?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            Task.FromResult(_lugares.FirstOrDefault(l => l.Id == id));

        public Task<Lugar?> ObterPorSlugAsync(string slug, CancellationToken cancellationToken = default) =>
            Task.FromResult(_lugares.FirstOrDefault(l => string.Equals(l.Slogan, slug, StringComparison.OrdinalIgnoreCase)));

        public Task<IReadOnlyList<Lugar>> BuscarAsync(PlaceSearchCriteria criterios, CancellationToken cancellationToken = default)
        {
            IEnumerable<Lugar> resultado = _lugares;
            if (criterios.Categoria.HasValue)
                resultado = resultado.Where(l => l.Categoria == criterios.Categoria.Value);
            return Task.FromResult<IReadOnlyList<Lugar>>(resultado.ToList());
        }

        public Task<IReadOnlyList<Lugar>> ObterFallbackCuradoriaAsync(int limite = 5, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<Lugar>>(_lugares.Take(limite).ToList());

        public Task AdicionarAsync(Lugar lugar, CancellationToken cancellationToken = default)
        {
            _lugares.Add(lugar);
            return Task.CompletedTask;
        }

        public Task AtualizarAsync(Lugar lugar, CancellationToken cancellationToken = default) => Task.CompletedTask;
    }

    [Fact]
    public async Task ListarLugaresAsync_DeveRetornarApenasLugaresAprovados_EMapearParaDto()
    {
        // 1. Arrange: cria fake com dados conhecidos
        var fakeRepo = new FakeLugarRepository([
            CriarLugar("Café Madeira", "cafe-madeira", StatusPublicacao.Aprovado),
            CriarLugar("Bar Provisório", "bar-provisorio", StatusPublicacao.EmRevisao)
        ]);
        var fakeEventoRepo = new FakeEventoRepository();
        var service = new CatalogoService(fakeRepo, fakeEventoRepo);

        // 2. Act: chama interface pública
        var resultados = await service.ListarLugaresAsync(new PlaceSearchCriteria());

        // 3. Assert: verifica o resultado observável pelo cliente
        var lugarDto = Assert.Single(resultados);
        Assert.Equal("cafe-madeira", lugarDto.Slug);
        Assert.Equal("aprovado", lugarDto.Status);
    }
}
```

### 2. Minimal API com `WebApplicationFactory<Program>` (Testes de Integração)

No projeto `YupAchei.IntegrationTests`, exercitamos o pipeline HTTP real da API sem precisar de Docker ou banco externo:

```csharp
// GOOD: Testa contrato HTTP real, status codes e serialização JSON
public class LocaisEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public LocaisEndpointsTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Get_Locais_DeveRetornarStatus200_E_ListaPopuladaPeloSeeder()
    {
        // Act
        var response = await _client.GetAsync("/api/locais");

        // Assert
        response.EnsureSuccessStatusCode(); // HTTP 200 OK
        var locais = await response.Content.ReadFromJsonAsync<List<LugarDto>>();

        Assert.NotNull(locais);
        Assert.NotEmpty(locais);
        Assert.Contains(locais, l => l.Slug == "complexo-estrada-de-ferro-madeira-mamore");
    }

    [Fact]
    public async Task Get_LocalPorIdOuSlugInexistente_DeveRetornarStatus404NotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/locais/slug-que-nao-existe");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
```
