#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Created on Sun Jun 26 17:03:24 2022

@author: lyd
"""

import csv
import sys
import math
import numpy as np
import matplotlib.pyplot as plt
from matplotlib import rcParams

rcParams.update({'figure.autolayout': True})
#csv.field_size_limit(sys.maxsize)

def read_HASH_vals(filename):
    length = 2
    repeats_stable = 0;
    repeats_set = 0;
    result_vector = np.zeros(length)
    with open(filename, mode='r') as file:
      reader = csv.reader(file, delimiter=',')
      for index, row in enumerate(reader):
          if "stable hash" in row[0] :
                  result_vector[0] = int(result_vector[0]) + int(row[1])
                  repeats_stable = repeats_stable + 1
          elif "set hash" in row[0] and "XOR" in row[1]:
                  result_vector[1] = int(result_vector[1]) + int(row[2])
                  repeats_set = repeats_set + 1
          elif "set hash" in row[0] and "ADD" in row[1]:
                  result_vector[1] = int(result_vector[1]) + int(row[2])
                  repeats_set = repeats_set + 1
    result_vector[0] = result_vector[0]/repeats_stable
    result_vector[1] = result_vector[1]/repeats_set
    return result_vector

def read_SYNC_vals(filename):
    repeats = 0;
    result_vector = np.zeros(1)
    with open(filename, mode='r') as file:
      reader = csv.reader(file, delimiter=',')
      for index, row in enumerate(reader):
          if index > 1:
              if not "LOG" in row[0] :
                      result_vector[0] = int(result_vector[0]) + int(row[0])
                      repeats = repeats + 1
    result_vector[0] = result_vector[0]/repeats
    return result_vector

def read_SYNC_vals_complete(filename):
    result_vector = []
    with open(filename, mode='r') as file:
      reader = csv.reader(file, delimiter=',')
      for index, row in enumerate(reader):
          if not "LOG" in row[0] :
              result_vector.append(int(row[0]))
    return np.array(result_vector)
        
def read_SYNCCOMM_vals_rounds(filename):
    result_vector = []
    with open(filename, mode='r') as file:
      reader = csv.reader(file, delimiter=',')
      for index, row in enumerate(reader):
          if "rounds" in row[0] :
                  result_vector.append(math.ceil(int(row[1])))
    return np.array(result_vector)
      

def read_SYNCCOMM_vals_steps(filename):
    result_vector = []
    with open(filename, mode='r') as file:
      reader = csv.reader(file, delimiter=',')
      for index, row in enumerate(reader):
          if "steps" in row[0]:
                  result_vector.append(int(row[1]))
    return np.array(result_vector)

def read_SYNCCOMM_vals_delta(filename):
    result_vector = []
    with open(filename, mode='r') as file:
      reader = csv.reader(file, delimiter=',')
      for index, row in enumerate(reader):
          if not "SUCCESSFUL TESTS" in row[0]:
                  result_vector.append(int(row[3]))
    return np.array(result_vector)
    
def read_SYNCCOMM_vals_n(filename):
    result_vector = []
    with open(filename, mode='r') as file:
      reader = csv.reader(file, delimiter=',')
      for index, row in enumerate(reader):
          if not "SUCCESSFUL TESTS" in row[0]:
                  result_vector.append(int(row[4]))
    return np.array(result_vector)

def read_SYNCCOMM_vals_nmin(filename):
    result_vector = []
    with open(filename, mode='r') as file:
      reader = csv.reader(file, delimiter=',')
      for index, row in enumerate(reader):
          if not "SUCCESSFUL TESTS" in row[0]:
                  result_vector.append(min(int(row[1]),int(row[2])))
    return np.array(result_vector)

     
def read_MEM_vals_entries(filename):
    result_vector = []
    with open(filename, mode='r') as file:
      reader = csv.reader(file, delimiter=',')
      for index, row in enumerate(reader):
          if not "MEMORY" in row[0] :
              if "entries" in row[1]:
                      result_vector.append(int(row[2]))
    return np.array(result_vector)
   
def read_MEM_vals_size(filename):
    result_vector = []
    with open(filename, mode='r') as file:
      reader = csv.reader(file, delimiter=',')
      for index, row in enumerate(reader):
          if not "MEMORY" in row[0] :
              if "size" in row[1]:
                      result_vector.append(int(row[2]))
    return np.array(result_vector)
    


def read_file(filename):
    result_vector = []
    with open(filename, mode='r') as file:
      for line in file:
        result_vector.append(line.split(','))
    return np.array(result_vector)


"""
comRoundsUpper = int(delta*int(2+2*math.ceil(math.log(nmin,b)) - math.floor(math.log(t, b))))+1
comComplexUpper = min(int(delta*int(2+2*math.ceil(math.log(nmin,b)) - math.floor(math.log(t, b))))+1, 2*n)
"""
def calc_SYNCCOMM_rounds_upper(nmin, b, t):
    result_vector = []
    length = nmin.size
    for i in range(length):
        #print("only: "+str(math.log2(i)))
        if nmin[i] == 0:
            result_vector.append(4)
        else:
            result_vector.append(int(2+2*math.ceil(math.log(nmin[i], b)) - math.floor(math.log(t, b)))+1)
    return np.array(result_vector)

def calc_SYNCCOMM_steps_upper(n, nmin, delta, b, t):
    result_vector = []
    length = n.size
    for i in range(length):
        first = 4
        if nmin[i] != 0:
             first = int(delta[i]*int(2+2*math.ceil(math.log(nmin[i], b)) - math.floor(math.log(t, b))))+1
        second = 2*n[i]
        #print("first "+str(first))
        #print("second "+str(second))
        result_vector.append( min(first, second) )
    return np.array(result_vector)




#hashes_1_vec = read_HASH_vals('skiplist_01/hashTimes.log')

factor = 1
b = 2
t = 1

"""
fig = plt.figure(1)
ax1 = fig.add_subplot(111)
ax1.bar(np.array(["peer"]), hashes_1_vec[0]/factor, width=0.5, color='navy')
ax1.set_ylabel("calculation time in milliseconds")
plt.title('Average stable hash calculation times')
plt.show()
fig.savefig('hashTimes.pdf')
fig.savefig('hashTimes.png')

fig = plt.figure(2)
ax1 = fig.add_subplot(111)
ax1.bar(np.array(["peer"]), hashes_1_vec[1]/factor, width=0.5, color='navy')
ax1.set_ylabel("calculation time in milliseconds")
plt.title('Average set fingerprint calculation times')
plt.show()
fig.savefig('fpTimes.pdf')
fig.savefig('fpTimes.png')
"""


mem_results = read_file("memResults.log")
mem_results2 = read_file("memResultsSet.log")

"""
0: TestName
1: axuDS
2: config
3: n0
4: n1
5: bytesReceived
6: bytesSent
7: complexity
8: rounds
9: time
10: nDelta
11: nUnion
"""
sync_results = read_file("testResults.log")



sync_1_vec_complete = np.array(sync_results[:,9]).astype(int)

syncComm_1_vec_rounds = np.array(sync_results[:,8]).astype(int)
syncComm_1_vec_steps = np.array(sync_results[:,7]).astype(int)
syncComm_1_vec_delta = np.array(sync_results[:,10]).astype(int)
syncComm_1_vec_n0 = np.array(sync_results[:,3]).astype(int)
syncComm_1_vec_n1 = np.array(sync_results[:,4]).astype(int)
syncComm_1_vec_bytesR = np.array(sync_results[:,5]).astype(int)
syncComm_1_vec_bytesS = np.array(sync_results[:,6]).astype(int)
syncComm_1_vec_bytesT = syncComm_1_vec_bytesR + syncComm_1_vec_bytesS
syncComm_1_vec_n = np.array(sync_results[:,11]).astype(int)
syncComm_1_vec_nmin = np.array([min(x, y) for x,y in zip(syncComm_1_vec_n0, syncComm_1_vec_n1)])
#syncComm_1_vec_nmin = np.min(syncComm_1_vec_n0, syncComm_1_vec_n1)

mem_1_vec_size = np.array(mem_results[:,1]).astype(int)
mem_1_vec_entries = np.array(mem_results[:,2]).astype(int)

mem_2_vec_size = np.array(mem_results2[:,1]).astype(int)
mem_2_vec_entries = np.array(mem_results2[:,2]).astype(int)


results_b2t1 = read_file("testResultsb2t1.log")
results_b3t2 = read_file("testResultsb3t2.log")
results_b6t4 = read_file("testResultsb6t4.log")
results_b100t1 = read_file("testResultsb100t1.log")
results_b2t100 = read_file("testResultsb2t100.log")
results_all = np.concatenate((results_b2t1, results_b3t2, results_b6t4, results_b100t1, results_b2t100))

results_b2t1Set = read_file("testResultsb2t1Set.log")

fixed_b2t1 =  results_b2t1[ np.char.find(results_b2t1[:,0], "Fixed") != -1]
fixed_b3t2 =  results_b3t2[ np.char.find(results_b2t1[:,0], "Fixed") != -1]
fixed_b6t4 =  results_b6t4[ np.char.find(results_b2t1[:,0], "Fixed") != -1]
fixed_b2t1Set = results_b2t1Set[ np.char.find(results_b2t1[:,0], "Fixed") != -1]

fixed_all = results_all[ np.char.find(results_all[:,0], "Fixed") != -1]
fixed_labels = fixed_all[:,0:3]
fixed_values = fixed_all[:,3:].astype(int)
fixed_unique_configs = list(set(fixed_labels[:,2]))

fixed_aux_all = np.concatenate((fixed_b2t1Set, fixed_b2t1))
fixed_aux_labels = fixed_aux_all[:,0:3]
fixed_aux_values = fixed_aux_all[:,3:].astype(int)
fixed_aux_unique_configs = [x for x in (set((fixed_aux_labels[:,1])))]
fixed_aux_unique_configs_small = [x[28:] for x in fixed_aux_unique_configs]
fig = plt.figure(3)
x1 = np.linspace(0,sync_1_vec_complete.size+1, sync_1_vec_complete.size)
ax1 = fig.add_subplot(111)
#ax1.scatter(x1, sync_1_vec_complete/factor, c='navy', marker="o", label='first peer')
#ax1.scatter(x2, sync_2_vec_complete/factor, c='coral', marker="o", label='second peer')
ax1.scatter(x1, sync_1_vec_complete/factor, c='navy', marker="o")
ax1.set_ylabel("synchronization time in milliseconds")
ax1.set_xlabel("test run id")
#plt.legend(loc='upper left');
plt.title('Synchronization times for branching factor 2')
plt.show()
fig.savefig('syncTimes.pdf')
fig.savefig('syncTimes.png')




fig = plt.figure(4)
ax1 = fig.add_subplot(111)
ax1.scatter(mem_1_vec_entries, mem_1_vec_size, c='navy', marker="o")
ax1.set_ylabel("necessary memory in bytes")
ax1.set_xlabel("number of entries")
plt.title('Necessary Memory for auxillary data structure')
plt.show()
fig.savefig('memSize.pdf')
fig.savefig('memSize.png')

fig = plt.figure(4)
ax1 = fig.add_subplot(111)
ax1.scatter(mem_2_vec_entries, mem_2_vec_size, c='navy', marker="o")
ax1.set_ylabel("necessary memory in bytes")
ax1.set_xlabel("number of entries")
plt.title('Necessary Memory for auxillary data structure')
plt.show()
fig.savefig('memSize2.pdf')
fig.savefig('memSize2.png')
"""
# compute slope m and intercept b
m, b = np.polyfit(mem_1_vec_entries, mem_1_vec_size, deg=1)

fig = plt.figure(44)
ax1 = fig.add_subplot(111)
ax1.scatter(mem_1_vec_entries, mem_1_vec_size, c='navy', marker="o")
plt.axline(xy1=(0, b), slope=m, color='yellowgreen', label=f'$y = {m:.2f}x {b:+.2f}$')
ax1.set_ylabel("necessary memory in bytes")
ax1.set_xlabel("number of entries")
plt.title('Necessary Memory for treap data structure')
plt.show()
fig.savefig('memSize_ref.pdf')
fig.savefig('memSize_ref.png')
"""


syncComm_1_vec_rounds_upper = calc_SYNCCOMM_rounds_upper(syncComm_1_vec_nmin, b, t)
syncComm_1_vec_steps_upper = calc_SYNCCOMM_steps_upper(syncComm_1_vec_n, syncComm_1_vec_nmin, syncComm_1_vec_delta, b, t)

fig = plt.figure(5)
x2 = np.linspace(0,syncComm_1_vec_rounds.size+1, syncComm_1_vec_rounds.size)
ax1 = fig.add_subplot(111)
ax1.scatter(x2, syncComm_1_vec_steps, c='navy', marker="o")
ax1.scatter(x2, syncComm_1_vec_steps_upper, c='darkorchid', marker="x")
ax1.set_ylabel("number of steps")
ax1.set_xlabel("test run id")
plt.yscale('log')
plt.title('Number of Steps for branching factor 2')
plt.show()
fig.savefig('syncSteps.pdf')
fig.savefig('syncSteps.png')

fig = plt.figure(6)
x2 = np.linspace(0,syncComm_1_vec_rounds.size+1, syncComm_1_vec_rounds.size)
ax1 = fig.add_subplot(111)
ax1.scatter(x2, syncComm_1_vec_rounds, c='navy', marker="o")
ax1.scatter(x2, syncComm_1_vec_rounds_upper, c='darkorchid', marker="x")
ax1.set_ylabel("number of rounds")
ax1.set_xlabel("test run id")
plt.title('Number of Rounds for branching factor 2')
plt.show()
fig.savefig('syncRounds.pdf')
fig.savefig('syncRounds.png')


fig = plt.figure(7)
ax1 = fig.add_subplot(111)
ax1.scatter(syncComm_1_vec_n, syncComm_1_vec_rounds, c='navy', marker="o")
ax1.scatter(syncComm_1_vec_n, syncComm_1_vec_rounds_upper, c='darkorchid', marker="x")
ax1.set_ylabel("number of rounds")
ax1.set_xlabel("n")
plt.title('Number of Rounds for branching factor 2')
plt.show()
fig.savefig('syncRounds_overn.pdf')
fig.savefig('syncRounds_overn.png')

fig = plt.figure(8)
ax1 = fig.add_subplot(111)
ax1.scatter(syncComm_1_vec_n, syncComm_1_vec_steps, c='navy', marker="o")
ax1.scatter(syncComm_1_vec_n, syncComm_1_vec_steps_upper, c='darkorchid', marker="x")
ax1.set_ylabel("number of steps")
ax1.set_xlabel("n")
plt.yscale('log')
plt.title('Number of Steps for branching factor 2')
plt.show()
fig.savefig('syncSteps_overn.pdf')
fig.savefig('syncSteps_overn.png')

fig = plt.figure(9)
ax1 = fig.add_subplot(111)
ax1.scatter(syncComm_1_vec_n, sync_1_vec_complete/factor, c='navy', marker="o")
ax1.set_ylabel("synchronization time in milliseconds")
ax1.set_xlabel("n")
#plt.legend(loc='upper left');
plt.title('Synchronization times for branching factor 2')
plt.show()
fig.savefig('syncTimes_overn.pdf')
fig.savefig('syncTimes_overn.png')

fig = plt.figure(9)
ax1 = fig.add_subplot(111)
ax1.scatter(syncComm_1_vec_delta, sync_1_vec_complete/factor, c='navy', marker="o")
ax1.set_ylabel("synchronization time in milliseconds")
ax1.set_xlabel("delta")
#plt.legend(loc='upper left');
plt.title('Synchronization times for branching factor 2')
plt.show()
fig.savefig('syncTimes_overdelta.pdf')
fig.savefig('syncTimes_overdelta.png')

fig = plt.figure(10)
ax1 = fig.add_subplot(111)
ax1.scatter(syncComm_1_vec_delta/syncComm_1_vec_n, sync_1_vec_complete/factor, c='navy', marker="o")
ax1.set_ylabel("synchronization time in milliseconds")
ax1.set_xlabel("delta/n")
#plt.legend(loc='upper left');
plt.title('Synchronization times for branching factor 2')
plt.show()
fig.savefig('syncTimes_overpercent.pdf')
fig.savefig('syncTimes_overpercent.png')


fig = plt.figure(11)
ax1 = fig.add_subplot(111)
ax1.scatter(syncComm_1_vec_delta, syncComm_1_vec_bytesT, c='navy', marker="o")
ax1.set_ylabel("Amount of bytes transmitted")
ax1.set_xlabel("delta")
#plt.legend(loc='upper left');
plt.title('Bytes transmitted for branching factor 2')
plt.show()
fig.savefig('syncBytes_overdelta.pdf')
fig.savefig('syncBytes_overdelta.png')

fig = plt.figure(12)
ax1 = fig.add_subplot(111)
ax1.scatter(syncComm_1_vec_delta, syncComm_1_vec_bytesT/1000/sync_1_vec_complete*1000 , c='navy', marker="o")
ax1.set_ylabel("Amount of Bandwidth Kbps needed (extrapolated)")
ax1.set_xlabel("delta")
#plt.legend(loc='upper left');
plt.title('Bandwith needed')
plt.show()
fig.savefig('syncBandwidth_overdelta.pdf')
fig.savefig('syncBandwidth_overdelta.png')


fig = plt.figure(13)
ax1 = fig.add_subplot(111)
for i in range(len(fixed_unique_configs)):
    idxs = fixed_labels[:,2] == fixed_unique_configs[i]
    ax1.scatter(fixed_values[idxs,7], fixed_values[idxs,2] + fixed_values[idxs,3], marker="o", label=fixed_unique_configs[i])
ax1.set_ylabel("Amount of bytes transmitted compared")
ax1.set_xlabel("delta")
plt.legend(loc='upper left');
plt.title('Bytes transmitted')
plt.show()
fig.savefig('syncBytes_overdeltacomp.pdf')
fig.savefig('syncBytes_overdeltacomp.png')

fig = plt.figure(14)
ax1 = fig.add_subplot(111)
for i in range(len(fixed_unique_configs)):
    idxs = fixed_labels[:,2] == fixed_unique_configs[i]
    ax1.scatter(fixed_values[idxs,7], fixed_values[idxs,4], marker="o", label=fixed_unique_configs[i])
plt.legend(loc='upper left');
ax1.set_ylabel("number of steps")
ax1.set_xlabel("delta")
plt.title('Number of Steps')
plt.show()
fig.savefig('syncSteps_overdeltacomp.pdf')
fig.savefig('syncSteps_overdeltacomp.png')


fig = plt.figure(15)
ax1 = fig.add_subplot(111)
for i in range(len(fixed_unique_configs)):
    idxs = fixed_labels[:,2] == fixed_unique_configs[i]
    ax1.scatter(fixed_values[idxs,7], fixed_values[idxs,5], marker="o", label=fixed_unique_configs[i])
plt.legend(loc='upper left');
ax1.set_ylabel("number of rounds")
ax1.set_xlabel("delta")
plt.title('Number of Rounds')
plt.show()
fig.savefig('syncRounds_overdeltacomp.pdf')
fig.savefig('syncRounds_overdeltacomp.png')

fig = plt.figure(16)
ax1 = fig.add_subplot(111)
for i in range(len(fixed_unique_configs)):
    idxs = fixed_labels[:,2] == fixed_unique_configs[i]
    ax1.scatter(fixed_values[idxs,7], fixed_values[idxs,6], marker="o", label=fixed_unique_configs[i])
plt.legend(loc='upper left');
ax1.set_ylabel("synchronization time in milliseconds")
ax1.set_xlabel("delta")
plt.title('Synchronization times')
plt.show()
fig.savefig('syncTimes_overdeltacomp.pdf')
fig.savefig('syncTimes_overdeltacomp.png')


fig = plt.figure(13)
ax1 = fig.add_subplot(111)
for i in range(len(fixed_unique_configs)):
    idxs = fixed_labels[:,2] == fixed_unique_configs[i]
    ax1.scatter(fixed_values[idxs,7], fixed_values[idxs,2] + fixed_values[idxs,3], marker="o", label=fixed_unique_configs[i])
ax1.set_ylabel("Amount of bytes transmitted")
ax1.set_xlabel("n")
plt.legend(loc='upper left');
plt.title('Bytes transmitted')
plt.show()
fig.savefig('syncBytes_overncomp.pdf')
fig.savefig('syncBytes_overncomp.png')

fig = plt.figure(17)
ax1 = fig.add_subplot(111)
for i in range(len(fixed_unique_configs)):
    idxs = fixed_labels[:,2] == fixed_unique_configs[i]
    ax1.scatter(fixed_values[idxs,8], (fixed_values[idxs,2]+fixed_values[idxs,3]) / fixed_values[idxs,6], marker="o", label=fixed_unique_configs[i])
ax1.set_ylabel("Amount of Bandwidth Kbps needed (extrapolated)")
ax1.set_xlabel("n")
plt.legend(loc='upper left');
plt.title('Bandwidth needed')
plt.show()
fig.savefig('syncBandwidth_overncomp.pdf')
fig.savefig('syncBandwidth_overncomp.png')

fig = plt.figure(17)
ax1 = fig.add_subplot(111)
for i in range(len(fixed_unique_configs)):
    idxs = fixed_labels[:,2] == fixed_unique_configs[i]
    ax1.scatter(fixed_values[idxs,7], (fixed_values[idxs,2]+fixed_values[idxs,3]) / fixed_values[idxs,6], marker="o", label=fixed_unique_configs[i])
ax1.set_ylabel("Amount of Bandwidth Kbps needed (extrapolated)")
ax1.set_xlabel("delta")
plt.legend(loc='upper left');
plt.title('Bandwidth needed')
plt.show()
fig.savefig('syncBandwidth_overdeltacomp.pdf')
fig.savefig('syncBandwidth_overdeltacomp.png')

fig = plt.figure(14)
ax1 = fig.add_subplot(111)
for i in range(len(fixed_unique_configs)):
    idxs = fixed_labels[:,2] == fixed_unique_configs[i]
    ax1.scatter(fixed_values[idxs,8], fixed_values[idxs,4], marker="o", label=fixed_unique_configs[i])
plt.legend(loc='upper left');
ax1.set_ylabel("number of steps")
ax1.set_xlabel("n")
plt.title('Number of Steps')
plt.show()
fig.savefig('syncSteps_overncomp.pdf')
fig.savefig('syncSteps_overncomp.png')


fig = plt.figure(15)
ax1 = fig.add_subplot(111)
for i in range(len(fixed_unique_configs)):
    idxs = fixed_labels[:,2] == fixed_unique_configs[i]
    ax1.scatter(fixed_values[idxs,8], fixed_values[idxs,5], marker="o", label=fixed_unique_configs[i])
plt.legend(loc='upper left');
ax1.set_ylabel("number of rounds")
ax1.set_xlabel("n")
plt.title('Number of Rounds')
plt.show()
fig.savefig('syncRounds_overncomp.pdf')
fig.savefig('syncRounds_overnacomp.png')

fig = plt.figure(16)
ax1 = fig.add_subplot(111)
for i in range(len(fixed_unique_configs)):
    idxs = fixed_labels[:,2] == fixed_unique_configs[i]
    ax1.scatter(fixed_values[idxs,8], fixed_values[idxs,6], marker="o", label=fixed_unique_configs[i])
plt.legend(loc='upper left');
ax1.set_ylabel("synchronization time in milliseconds")
ax1.set_xlabel("n")
plt.title('Synchronization times')
plt.show()
fig.savefig('syncTimes_overncomp.pdf')
fig.savefig('syncTimes_overncomp.png')


bar_width = 0.0
idxs = fixed_aux_labels[:,1] == fixed_aux_unique_configs[0]
print(fixed_aux_values[idxs,7].shape)
print(fixed_aux_values[idxs,7].shape)
x = np.arange(fixed_aux_values[idxs,7].shape[0])

print(x.shape)
print(fixed_aux_values[idxs,7].shape)
fig = plt.figure(17)
ax1 = fig.add_subplot(111)
for i in range(len(fixed_aux_unique_configs)):
    idxs = fixed_aux_labels[:,1] == fixed_aux_unique_configs[i]
    ax1.scatter(fixed_aux_values[idxs,7], fixed_aux_values[idxs,6], label=fixed_aux_unique_configs_small[i])
    #ax1.bar(x + i * bar_width, fixed_aux_values[idxs,6], label=fixed_aux_unique_configs_small[i])
#plt.xticks([r + bar_width for r in range(len(x))],fixed_aux_values[idxs,7] )
plt.legend(loc='upper left');
ax1.set_ylabel("synchronization time in milliseconds")
ax1.set_xlabel("delta")
plt.title('Synchronization times')
plt.show()
fig.savefig('syncTimes_overdeltacompaux.pdf')
fig.savefig('syncTimes_overdeltacompaux.png')