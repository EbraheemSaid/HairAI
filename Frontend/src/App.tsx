import { RouterProvider } from "react-router-dom";
import { Toaster } from "react-hot-toast";
import { router } from "./routes/router";

function App() {
  return (
    <div className="App">
      <RouterProvider router={router} />
      <Toaster position="top-right" />
    </div>
  );
}

export default App;
