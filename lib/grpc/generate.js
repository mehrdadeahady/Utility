const path = require("path");
const { execSync } = require("child_process");

// __dirname = C:\Users\User\Desktop\Utility\lib\grpc
const root = path.join(__dirname, "..", ".."); // C:\Users\User\Desktop\Utility

const protoDir = path.join(__dirname, "protos");
const protoFile = path.join(protoDir, "ip_service.proto");
const outDir = path.join(__dirname, "generated");

// Correct Windows paths to protoc + TS plugin
const protoc = path.join(root, "node_modules", ".bin", "grpc_tools_node_protoc.cmd");
const protocGenTs = path.join(root, "node_modules", ".bin", "protoc-gen-ts.cmd");

// Generate JS + gRPC service stubs
execSync(
  `"${protoc}" --js_out=import_style=commonjs,binary:"${outDir}" --grpc_out=grpc_js:"${outDir}" --proto_path="${protoDir}" "${protoFile}"`,
  { stdio: "inherit" }
);

// Generate TypeScript definitions
execSync(
  `"${protoc}" --plugin=protoc-gen-ts="${protocGenTs}" --ts_out="${outDir}" --proto_path="${protoDir}" "${protoFile}"`,
  { stdio: "inherit" }
);
