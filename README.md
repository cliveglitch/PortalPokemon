# Prueba Tecnica Portal Pokemon
Aplicación web en .NET Core 8 MVC que muestra un listado de Pokémon's mediante el consumo de la API https://pokeapi.co/

## Funcionalidades
Filtros por nombre y especie, paginación, cacheo de datos, vista de detalle de Pokémon, exportar grid como un excel y exportación de grid a correo.

Se utilizó la interfaz IMemoryCache con el paquete Microsoft.Extensions.Caching.Memory para manejar cache en la memoria de la aplicación por ser simple y fácil de implementar, algo como HybridCache puede manejar requests simultaneous al mismo tiempo pero no se espera tener tantos usuarios.

Se utilizó el paquete ClosedXML para exportar el Excel de la pagina actual por si simpleza para ser implementado y su una licencia de uso libre, a diferencia de otras alternativas como EPPlus que requiere configuración para usarlo o Open-XML-SDK que requiere mas código para poder hacer la misma tarea mejor performance, pero con lo poco que se va a exportar esas ganancias no valen la pena.