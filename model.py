# TC2008B Modelación de Sistemas Multiagentes con gráficas computacionales
# Python server to interact with Unity via POST
# Sergio Ruiz-Loza, Ph.D. March 2021

from http.server import BaseHTTPRequestHandler, HTTPServer
import logging
import json

# Importamos las clases que se requieren para manejar los agentes (Agent) y su entorno (Model).
# Cada modelo puede contener múltiples agentes.
from mesa import Agent, Model

from mesa.batchrunner import batch_run

from mesa.space import PropertyLayer

# Debido a que necesitamos que existe un solo agente por celda, elegimos ''SingleGrid''.
from mesa.space import SingleGrid

# Con ''RandomActivation'', hacemos que todos los agentes se activen ''al mismo tiempo''.
from mesa.time import RandomActivation

# Con ''RandomActivation'', hacemos que todos los agentes se activen ''al mismo tiempo''.
from mesa.time import BaseScheduler

# Haremos uso de ''DataCollector'' para obtener información de cada paso de la simulación.
from mesa.datacollection import DataCollector

# matplotlib lo usaremos crear una animación de cada uno de los pasos
# del modelo.
#%matplotlib inline
import matplotlib
import matplotlib.pyplot as plt
import matplotlib.animation as animation
plt.rcParams["animation.html"] = "jshtml"
matplotlib.rcParams['animation.embed_limit'] = 2**128

# seaborn lo usaremos desplegar una gráficas más ''vistosas'' de
# nuestro modelo
import seaborn as sns

import random


# Importamos los siguientes paquetes para el mejor manejo de valores numéricos.
import numpy as np
import pandas as pd

# Definimos otros paquetes que vamos a usar para medir el tiempo de ejecución de nuestro algoritmo.
import time
import datetime

#Clase de los dinosaurios
class Dino(Agent):
    def __init__(self, unique_id, model):
        super().__init__(unique_id, model)
        self.id = unique_id
        self.position = self.pos
        self.isCarring = False
        self.advancePoints = 4
        self.addedAdvancePoints = 0


    def step(self):
      print("agente: {} pos: {}".format(self.id, self.pos))
      self.move()

    def move(self):
      if self.isCarring:
        self.actionMoveVictim()
      else:
        ap = np.random.randint(1, (self.advancePoints + self.addedAdvancePoints)+1)
        self.isPOI()
        self.isFire()
        self.actionMove(1)
        self.position = self.pos



    def actionMoveVictim(self):



      pass

    def actionMove(self, ap):
      for i in range(ap):
        neighborhood = self.model.grid.get_neighborhood(self.pos, moore=False, include_center=False)
        agents = list(self.model.grid.get_neighbors(self.pos, moore=False, include_center=False))
        temp = []
        for neighbor in neighborhood:
            temp.append(neighbor)

        #Borra de la variable temporal las casillas en las que coincida que existan agentes
        for agent in agents:
            if agent.pos in temp:
                del temp[temp.index(agent.pos)]

        random.shuffle(temp)
        #print(temp)
        for i in temp:
          flag = self.isWall(self.pos, i)
          #print(flag)
          if flag:
            #print("Se movio hacia {}".format(i))
            break

    def isFire(self):
      posicion = tuple([self.pos[0]+1, self.pos[1]+1])
      if posicion in self.model.fire:
        #print("Casilla en llamas")
        #print(self.model.fire)
        del self.model.fire[self.model.fire.index(posicion)] 
          


    def isDoor(self, next, flag):
      if flag == 0:
        for door in self.model.doors:
          objective = tuple([door[0][0]-1, door[0][1]-1])
          casilla = tuple([door[1][0]-1, door[1][1]-1])
          if self.pos == casilla and next == objective:
            #print("Abrio la concha abajo hacia arriba")
            return True
      elif flag == 1:
        for door in self.model.doors:
          casilla = tuple([door[0][0]-1, door[0][1]-1])
          objective = tuple([door[1][0]-1, door[1][1]-1])
          if self.pos == casilla and next == objective:
            #print("Abrio la concha arriba hacia abajo")
            return True
      elif flag == 2:
        for door in self.model.doors:
          casilla = tuple([door[0][0]-1, door[0][1]-1])
          objective = tuple([door[1][0]-1, door[1][1]-1])
          if self.pos == casilla and next == objective:
            #print("Abrio la concha izq hacia derecha")
            return True
      elif flag == 3:
        for door in self.model.doors:
          objective = tuple([door[0][0]-1, door[0][1]-1])
          casilla = tuple([door[1][0]-1, door[1][1]-1])
          if self.pos == casilla and next == objective:
            #print("Abrio la concha derecha hacia izq")
            return True
      
      return False

    def isPOI(self):
      for poi in self.model.poi:
        if self.pos == poi[0]:
          print("True")

    def isWall(self, cell, next):
      direction = self.model.map[cell[0]][cell[1]]
      result = ((cell[0]-next[0]), (cell[1]-next[1]))
      

      #Movimiento abajo hacia arriba
      if result == (1, 0):
        if direction[0] == "1": # Si hay un muro en la direcion a moverse
          if self.isDoor(next, 0): #Si hay una puerta en la celda
            #Modifica los valores de las paredes en el mapa, abriendo la puerta
            self.model.map[cell[0]][cell[1]] = direction[:-2] + "0" + direction[-1]
            self.model.map[next[0]][next[1]] = "0" + direction[1:]

            #Se mueve
            self.model.grid.move_agent(self, next)
            return True
          else:
            return False
        else:
          self.model.grid.move_agent(self, next)
          return True

      #Movimiento derecha a izquierda
      elif result == (0, 1):
        if direction[1] == "1":
          if self.isDoor(next, 3): #Si hay una puerta en la celda
            #Modifica los valores de las paredes en el mapa, abriendo la puerta
            self.model.map[cell[0]][cell[1]] = direction[0] + "0" + direction[2:]
            self.model.map[next[0]][next[1]] = direction[:-1] + "0"

            #Se mueve
            self.model.grid.move_agent(self, next)
            return True
          else:
            return False
        else:
          self.model.grid.move_agent(self, next)
          return True

      #Movimiento arriba hacia abajo
      elif result == (-1, 0): #Horizontal
        if direction[2] == "1":
          if self.isDoor(next, 1): #Si hay una puerta en la celda
            #Modifica los valores de las paredes en el mapa, abriendo la puerta
            self.model.map[cell[0]][cell[1]] = "0" + direction[1:]
            self.model.map[next[0]][next[1]] = direction[:-2] + "0" + direction[-1]

            #Se mueve
            self.model.grid.move_agent(self, next)
            return True
          else:
            return False
        else:
          self.model.grid.move_agent(self, next)
          return True

      #Movimiento izq a derecha
      elif result == (0, -1):
        if direction[3] == "1":#Vertical
          if self.isDoor(next, 2): #Si hay una puerta en la celda
            #Modifica los valores de las paredes en el mapa, abriendo la puerta
            self.model.map[cell[0]][cell[1]] = direction[:-1] + "0"
            self.model.map[next[0]][next[1]] = direction[0] + "0" + direction[2:]

            #Se mueve
            self.model.grid.move_agent(self, next)
            return True
          else:
            return False
        else:
          self.model.grid.move_agent(self, next)
          return True
      else:
        return False


def get_grid(model):
    grid = np.zeros( (model.grid.width, model.grid.height) )
    for (content, (x, y)) in model.grid.coord_iter():
        if (content):
            grid[x][y] = 1
        elif (x+1, y+1) in model.fire:
          grid[x][y] = 2
        elif (x+1, y+1) in model.smokes:
          grid[x][y] = 3
        else:
            grid[x][y] = 0

    return grid

#Modelo del tablero
class Board(Model):
  def __init__(self, width, height, array):
    self.grid = SingleGrid(width, height, True)
    self.schedule = BaseScheduler(self)
    self.map = array[0] #Mapa de las paredes del tablero
    self.poi = array[1] #Puntos de interes
    self.fire = array[2] #Casillas con fuego
    self.doors = array[3] #Casillas con puertas
    self.entrance = array[4] #Casillas con entradas
    self.smokes = [] #Casillas con humo

    self.datacollector = DataCollector(
      model_reporters = {"Map": "map", "POI": "poi", "Fire": "fire", "Smokes": "smokes"}, agent_reporters = {"Pos": "pos"}
    )
    """
    self.datacollector = DataCollector(
      model_reporters = {"Grid": get_grid}
    )
    """

    #Crea los agentes
    for i in range(0, 6, 1):
        agent = Dino(i, self)
        self.grid.move_to_empty(agent)
        #self.grid.place_agent(agent, (5, 7))
        self.schedule.add(agent)

  def step(self):
    print("\nStep: " + str(self.schedule.steps))
    self.datacollector.collect(self)
    self.schedule.step()
    self.diceRun()
    print("Fire: " + str(self.fire))
    print("Smoke: " + str(self.smokes))

  def diceRun(self):
      dice1 = np.random.randint(1,7)
      dice2 = np.random.randint(1,7)
      newFire = tuple([dice1, dice2])

      if newFire in self.smokes:
        print("se reemplazo")
        self.fire.append(newFire)
        self.smokes.remove(newFire)
      else:
        self.smokes.append(newFire)

WIDTH = 6
HEIGHT = 8
configInicial = [[["1100", "1000", "1001", "1100", "1001", "1100", "1000", "1001"], ["0100", "0000", "0011", "0100", "0011", "0110", "0010", "0011"], ["0100", "0001", "1100", "1000", "1000", "1001", "1100", "1001"], ["0100", "0011", "0110", "0010", "0010", "0011", "0110", "0011"], ["1100", "1000", "1000", "1000", "1001", "1100", "1001", "1101"], ["0110", "0010", "0010", "0010", "0011", "0110", "0011", "0111"]],
                 [[(2, 4), True], [(5, 1), False], [(5, 8), True]],
                 [(2, 2), (2, 3), (3, 2), (3, 3), (3, 4), (3, 5), (4, 4), (5, 6), (5, 7), (6, 6)],
                 [[(1, 3), (1, 4)], [(2, 5), (2, 6)], [(2, 8), (3, 8)], [(3, 2), (3, 3)], [(4, 4), (5, 4)], [(4, 6), (4, 7)], [(6, 5), (6, 6)], [(6, 7), (6, 8)]],
                 [(1, 6), (3, 1), (4, 8), (6, 3)]]
model = Board(WIDTH, HEIGHT, configInicial)
STEPS = 30

"""
for i in range(STEPS):
  model.step()
model.step()

infoBomberos = model.datacollector.get_agent_vars_dataframe().to_json()
print(infoBomberos)
infoTablero = model.datacollector.get_model_vars_dataframe().to_json()
print(infoTablero)


# Obtenemos la información que almacenó el colector, este nos entregará 
# un DataFrame de pandas que contiene toda la información.
all_grid = model.datacollector.get_model_vars_dataframe()
print(all_grid)
# Graficamos la información usando `matplotlib`
# %%capture

fig, axs = plt.subplots(figsize=(7,7))
axs.set_xticks([])
axs.set_yticks([])
patch = plt.imshow(all_grid.iloc[0,0], cmap=plt.cm.tab20)

def animate(i):
    patch.set_data(all_grid.iloc[i,0])

anim = animation.FuncAnimation(fig, animate, frames=STEPS)
"""

class Server(BaseHTTPRequestHandler):
    
    def _set_response(self):
        self.send_response(200)
        self.send_header('Content-type', 'text/html')
        self.end_headers()
        
    def do_GET(self):
        self._set_response()
        self.wfile.write("Get".format(0).encode('utf-8'))

    def do_POST(self):
        model.step()
        infoBomberos = model.datacollector.get_agent_vars_dataframe().to_json()
        infoTablero = model.datacollector.get_model_vars_dataframe().to_json()
        self._set_response()
        self.wfile.write("{} {}".format(infoBomberos,infoTablero).encode('utf-8'))


def run(server_class=HTTPServer, handler_class=Server, port=8585):
    logging.basicConfig(level=logging.INFO)
    server_address = ('', port)
    httpd = server_class(server_address, handler_class)
    logging.info("Starting httpd...\n") # HTTPD is HTTP Daemon!
    try:
        httpd.serve_forever()
    except KeyboardInterrupt:   # CTRL+C stops the server
        pass
    httpd.server_close()
    logging.info("Stopping httpd...\n")

if __name__ == '__main__':
    from sys import argv
    
    if len(argv) == 2:
        run(port=int(argv[1]))
    else:
        run()