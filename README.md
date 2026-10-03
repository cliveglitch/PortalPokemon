# Prueba Tecnica Portal Pokemon
Aplicación web en .NET Core 8 MVC que muestra un listado de pokemones mediante el consumo de la API https://pokeapi.co/ 

## Funcionalidades
Filtros por nombre y especie, paginación, cacheo de datos, vista de detalle de pokemon, exportar grid a excel y correo.

Se utilizó la interfaz IMemoryCache con el paquete Microsoft.Extensions.Caching.Memory para manejar cache en la memoria de la apliación por ser simple y facil de implementar, algo como HybridCache puede manejar requests simultanios al mismo tiempo pero no se espera tener tantos usuarios.

