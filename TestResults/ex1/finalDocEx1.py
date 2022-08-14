#!/usr/bin/env python3
# -*- coding: utf-8 -*-

import csv
import math
import numpy as np
import matplotlib.pyplot as plt
from matplotlib import rcParams

rcParams.update({'figure.autolayout': True})

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
        result_vector.append( min(first, second) )
    return np.array(result_vector)





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



results_all = read_file("testResultsTrivial.log")
results_all = results_all[results_all[:,0].argsort()]
results_labels = results_all[:,0:3]
results_values = results_all[:,3:].astype(int)
results_all_unique_configs = list(set(results_labels[:,2]))
results_all_unique_names = list(set(results_labels[:,0]))


print(len(results_all_unique_names))


print(results_all)
x_pos = np.arange(1, 2 * len(results_all_unique_names), 2)
bar_width = 0.3
idxs = results_labels[:,1] == results_all_unique_configs[0]

x = np.arange(results_values[idxs,7].shape[0])

fig = plt.figure(1)
ax1 = fig.add_subplot()
for i in range(len(results_all_unique_configs)):
    idxs = results_labels[:,2] == results_all_unique_configs[i]
    arr = results_values[idxs,:]
    uniques = np.unique(results_labels[idxs,0], return_index=True)
    arr2 = np.asarray(np.split(arr[:,6], uniques[1][1:]))
    ax1.bar(x_pos + i * bar_width,  np.mean(arr2, axis=1), label=results_all_unique_configs[i],
            width = bar_width, yerr=np.std(arr2, axis=1))
    plt.xticks(x_pos + bar_width, uniques[0], rotation=45)
plt.legend(loc='upper left');
ax1.set_ylabel("synchronization time in milliseconds")
ax1.set_xlabel("Test cases")
plt.title('Synchronization times')
plt.show()
fig.savefig('syncTimes_EX1.png')

fig = plt.figure(2)
ax1 = fig.add_subplot()
for i in range(len(results_all_unique_configs)):
    idxs = results_labels[:,2] == results_all_unique_configs[i]
    arr = results_values[idxs,:]
    uniques = np.unique(results_labels[idxs,0], return_index=True)
    arr2 = np.asarray(np.split(arr[:,5],uniques[1][1:]))
    ax1.bar(x_pos + i * bar_width,  np.mean(arr2, axis=1), label=results_all_unique_configs[i],
            width = bar_width, yerr=np.std(arr2, axis=1))
    plt.xticks(x_pos + bar_width, uniques[0], rotation=45)
plt.legend(loc='upper left');
ax1.set_ylabel("number of communication rounds needed")
ax1.set_xlabel("Test cases")
plt.title('Communication Rounds')
plt.show()
fig.savefig('rounds_EX1.png')

fig = plt.figure(3)
ax1 = fig.add_subplot()
for i in range(len(results_all_unique_configs)):
    idxs = results_labels[:,2] == results_all_unique_configs[i]
    arr = results_values[idxs,:]
    uniques = np.unique(results_labels[idxs,0], return_index=True)
    arr2 = np.asarray(np.split(arr[:,4], uniques[1][1:]))
    ax1.bar(x_pos + i * bar_width,  np.mean(arr2, axis=1), label=results_all_unique_configs[i],
            width = bar_width, yerr=np.std(arr2, axis=1))
    plt.xticks(x_pos + bar_width, uniques[0], rotation=45)
plt.legend(loc='upper left');
ax1.set_ylabel("complexity needed for synchronization")
ax1.set_xlabel("Test cases")
plt.title('Complexity')
plt.show()
fig.savefig('complex_EX1.png')

fig = plt.figure(4)
ax1 = fig.add_subplot()
for i in range(len(results_all_unique_configs)):
    idxs = results_labels[:,2] == results_all_unique_configs[i]
    arr = results_values[idxs,:]
    uniques = np.unique(results_labels[idxs,0], return_index=True)
    arr2 = np.asarray(np.split(arr[:,3] + arr[:,2], uniques[1][1:]))
    ax1.bar(x_pos + i * bar_width,  np.mean(arr2, axis=1), label=results_all_unique_configs[i],
            width = bar_width, yerr=np.std(arr2, axis=1))
    plt.xticks(x_pos + bar_width, uniques[0], rotation=45)
plt.legend(loc='upper left');
ax1.set_ylabel("number of bytes transmitted")
ax1.set_xlabel("Test cases")
plt.title('Bytes transmitted')
plt.show()
fig.savefig('bytes_EX1.png')