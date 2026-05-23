import { Board } from './components/Board';
import { Header } from './components/Header';
import { ToastContainer } from './components/Toast';

function App() {
  return (
    <div className="app">
      <ToastContainer />
      <Header />
      <Board />
    </div>
  );
}

export default App;
