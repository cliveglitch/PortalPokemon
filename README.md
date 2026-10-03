# Prueba Tecnica Portal Pokemon
Aplicación web en .NET Core 8 MVC que muestra un listado de Pokémon's mediante el consumo de la API https://pokeapi.co/

## Funcionalidades
Se muestra un grid de máximo 30 pokemon por pagina.
Tiene filtros por nombre y especie, paginación, cacheo de datos, vista de detalle de Pokémon, exportar grid como un excel y envío de correo un pokemon individual o el grid mostrado.

Se utilizó la interfaz IMemoryCache con el paquete Microsoft.Extensions.Caching.Memory para manejar cache en la memoria de la aplicación por ser simple y fácil de implementar, algo como HybridCache puede manejar requests simultaneous al mismo tiempo pero no se espera tener tantos usuarios.

Se utilizó el paquete ClosedXML para exportar el Excel de la pagina actual por si simpleza para ser implementado y su una licencia de uso libre, a diferencia de otras alternativas como EPPlus que requiere configuración para usarlo o Open-XML-SDK que requiere mas código para poder hacer la misma tarea mejor performance, pero con lo poco que se va a exportar esas ganancias no valen la pena.

## Configuración de envío de correos

Para poder probarlo busca y abre el archivo appsettings.json y en el objeto Smtp puedes llenar la información necesaria, dejar la propiedad "UseAuthentication" en false si se va a usar un servidor de prueba como smtp4dev que no requiere autenticación.

Si no se va a usar un servidor de prueba puede cambiar la propiedad "UseAuthentication" a true y poner las credenciales de usuario y contraseña en un secrets.json que se puede encontrar en Visual Studio al darle click derecho al proyecto PortalPokemon y luego a la opción "Administrar secretos de usuario" que va a abrir el antes mencionado archivo, ahi ya puedes agregar el siguiente objeto llenando sus propiedades con tus credenciales:

"Smtp": {
  "Username": "",
  "Password": ""
}