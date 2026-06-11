import servicesConfig from "../config/services.json";

export type ServiceEntry = {
  name: string;
  address: string;
  proto: string;
  route: string;
};

export function loadServices(): ServiceEntry[] {
  return servicesConfig.services.map((svc) => {
    let route = svc.name.toLowerCase();

    // Fix IPService → ipservice
    if (route === "ipservice") {
      route = "ipservice";
    }

    // Fix BypassCorsService → bypasscors
    if (route === "bypasscorsservice") {
      route = "bypasscors";
    }

    return {
      ...svc,
      route
    };
  });
}

export function getService(name: string): ServiceEntry | undefined {
  return loadServices().find(
    (svc) => svc.route.toLowerCase() === name.toLowerCase()
  );
}
