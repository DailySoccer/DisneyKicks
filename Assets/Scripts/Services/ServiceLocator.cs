using System;
using System.Collections.Generic;

public static class ServiceLocator {

  private static Dictionary<Type, Object> registry = new Dictionary<Type, Object>();

  /// <summary>
  /// Register a new service to offer clients.
  /// </summary>
  /// <typeparam name="T">Type to store the service as.</typeparam>
  /// <param name="service">Registered service.</param>
  public static void Register<T>(T service) {
    Type type = typeof( T );

    if (null == service) {
      Remove<T>();
    } else {
      if (registry.ContainsKey( type )) {
        Remove<T>();
      }

      _Register( type, service );
    }
  }

  /// <summary>
  /// Add a new service to the registry.
  /// </summary>
  /// <param name="type">Type to insert the service as.</param>
  /// <param name="service">Service to add.</param>
  private static void _Register(Type type, Object service) {
    registry.Add( type, service );
  }

  /// <summary>
  /// Request a service of a given type.
  /// </summary>
  /// <typeparam name="T">
  /// The desired service type.
  /// </typeparam>
  /// <returns>The requested service.</returns>
  /// <exception cref="ArgumentException">
  /// If no service of type <code>T</code> has been registered.
  /// </exception>
  public static T Request<T>() where T : class {
    Type type = typeof( T );

    if (!registry.ContainsKey( type )) {
      throw new ArgumentException(
          "No service of type '" + type + "' has been registered.",
          "type" );
    }

    return (T)registry[type];
  }

  /// <summary>
  /// Remove an registered service.
  /// </summary>
  /// <param name="type">Service type to remove.</param>
  public static void Remove<T>() {
    Type type = typeof( T );
    if (registry.ContainsKey( type )) {
      registry.Remove( type );
    }
  }

  /// <summary>
  /// Clear all the registered services.
  /// </summary>
  public static void Clear() {
    registry.Clear();
  }
}
