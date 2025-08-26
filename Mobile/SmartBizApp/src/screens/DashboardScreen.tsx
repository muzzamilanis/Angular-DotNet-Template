import React, { useEffect, useState } from 'react';
import {
  View,
  Text,
  StyleSheet,
  ScrollView,
  RefreshControl,
  TouchableOpacity,
  Alert,
} from 'react-native';
import { Ionicons } from '@expo/vector-icons';
import { useAuth } from '../utils/AuthContext';
import ApiService from '../services/ApiService';
import { Company, Project, SaleOrder } from '../types';

const DashboardScreen: React.FC = () => {
  const { user, logout } = useAuth();
  const [isRefreshing, setIsRefreshing] = useState(false);
  const [stats, setStats] = useState({
    totalCompanies: 0,
    totalProjects: 0,
    totalOrders: 0,
    pendingOrders: 0,
  });

  useEffect(() => {
    loadDashboardData();
  }, []);

  const loadDashboardData = async () => {
    try {
      setIsRefreshing(true);
      
      // Load basic stats - in a real app, you might have specific dashboard endpoints
      const [companies, projects, orders] = await Promise.allSettled([
        ApiService.getCompanies(),
        ApiService.getProjects(),
        ApiService.getSaleOrders(),
      ]);

      setStats({
        totalCompanies: companies.status === 'fulfilled' ? companies.value.length : 0,
        totalProjects: projects.status === 'fulfilled' ? projects.value.length : 0,
        totalOrders: orders.status === 'fulfilled' ? orders.value.length : 0,
        pendingOrders: orders.status === 'fulfilled' 
          ? orders.value.filter(order => order.status.toLowerCase() === 'pending').length 
          : 0,
      });
    } catch (error) {
      console.error('Error loading dashboard data:', error);
      Alert.alert('Error', 'Failed to load dashboard data');
    } finally {
      setIsRefreshing(false);
    }
  };

  const handleLogout = () => {
    Alert.alert(
      'Logout',
      'Are you sure you want to logout?',
      [
        { text: 'Cancel', style: 'cancel' },
        { text: 'Logout', style: 'destructive', onPress: logout },
      ]
    );
  };

  const StatCard: React.FC<{
    icon: keyof typeof Ionicons.glyphMap;
    title: string;
    value: number;
    color: string;
  }> = ({ icon, title, value, color }) => (
    <View style={[styles.statCard, { borderLeftColor: color }]}>
      <View style={styles.statCardContent}>
        <View style={styles.statCardLeft}>
          <Text style={styles.statCardValue}>{value}</Text>
          <Text style={styles.statCardTitle}>{title}</Text>
        </View>
        <View style={[styles.statCardIcon, { backgroundColor: color }]}>
          <Ionicons name={icon} size={24} color="#fff" />
        </View>
      </View>
    </View>
  );

  const QuickActionCard: React.FC<{
    icon: keyof typeof Ionicons.glyphMap;
    title: string;
    description: string;
    onPress: () => void;
  }> = ({ icon, title, description, onPress }) => (
    <TouchableOpacity style={styles.quickActionCard} onPress={onPress}>
      <View style={styles.quickActionIcon}>
        <Ionicons name={icon} size={24} color="#667eea" />
      </View>
      <View style={styles.quickActionContent}>
        <Text style={styles.quickActionTitle}>{title}</Text>
        <Text style={styles.quickActionDescription}>{description}</Text>
      </View>
      <Ionicons name="chevron-forward" size={20} color="#ccc" />
    </TouchableOpacity>
  );

  return (
    <ScrollView 
      style={styles.container}
      refreshControl={
        <RefreshControl refreshing={isRefreshing} onRefresh={loadDashboardData} />
      }
    >
      {/* Header */}
      <View style={styles.header}>
        <View>
          <Text style={styles.welcomeText}>Welcome back,</Text>
          <Text style={styles.userNameText}>{user?.fullName || user?.userName}</Text>
          {user?.company && (
            <Text style={styles.companyText}>{user.company.name}</Text>
          )}
        </View>
        <TouchableOpacity style={styles.logoutButton} onPress={handleLogout}>
          <Ionicons name="log-out-outline" size={24} color="#667eea" />
        </TouchableOpacity>
      </View>

      {/* Stats Cards */}
      <View style={styles.statsContainer}>
        <Text style={styles.sectionTitle}>Overview</Text>
        <View style={styles.statsGrid}>
          <StatCard
            icon="business-outline"
            title="Companies"
            value={stats.totalCompanies}
            color="#667eea"
          />
          <StatCard
            icon="folder-outline"
            title="Projects"
            value={stats.totalProjects}
            color="#28a745"
          />
          <StatCard
            icon="receipt-outline"
            title="Total Orders"
            value={stats.totalOrders}
            color="#17a2b8"
          />
          <StatCard
            icon="time-outline"
            title="Pending Orders"
            value={stats.pendingOrders}
            color="#ffc107"
          />
        </View>
      </View>

      {/* Quick Actions */}
      <View style={styles.quickActionsContainer}>
        <Text style={styles.sectionTitle}>Quick Actions</Text>
        
        {user?.isSuperAdmin && (
          <QuickActionCard
            icon="people-outline"
            title="Manage Users"
            description="Add, edit, or manage user accounts"
            onPress={() => {/* Navigate to Users screen */}}
          />
        )}
        
        <QuickActionCard
          icon="business-outline"
          title="View Companies"
          description="Browse and manage company information"
          onPress={() => {/* Navigate to Companies screen */}}
        />
        
        <QuickActionCard
          icon="folder-outline"
          title="Projects"
          description="View and manage project details"
          onPress={() => {/* Navigate to Projects screen */}}
        />
        
        <QuickActionCard
          icon="receipt-outline"
          title="Sale Orders"
          description="Process and track sale orders"
          onPress={() => {/* Navigate to Orders screen */}}
        />
      </View>

      {/* User Info */}
      <View style={styles.userInfoContainer}>
        <Text style={styles.sectionTitle}>Account Information</Text>
        <View style={styles.userInfoCard}>
          <View style={styles.userInfoRow}>
            <Text style={styles.userInfoLabel}>Email:</Text>
            <Text style={styles.userInfoValue}>{user?.email}</Text>
          </View>
          <View style={styles.userInfoRow}>
            <Text style={styles.userInfoLabel}>Role:</Text>
            <Text style={styles.userInfoValue}>
              {user?.isSuperAdmin ? 'Super Admin' : 'User'}
            </Text>
          </View>
          {user?.company && (
            <View style={styles.userInfoRow}>
              <Text style={styles.userInfoLabel}>Company:</Text>
              <Text style={styles.userInfoValue}>{user.company.name}</Text>
            </View>
          )}
        </View>
      </View>
    </ScrollView>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#f8f9fa',
  },
  header: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    padding: 20,
    backgroundColor: '#fff',
    borderBottomWidth: 1,
    borderBottomColor: '#e1e1e1',
  },
  welcomeText: {
    fontSize: 16,
    color: '#666',
  },
  userNameText: {
    fontSize: 22,
    fontWeight: 'bold',
    color: '#333',
  },
  companyText: {
    fontSize: 14,
    color: '#667eea',
    fontWeight: '500',
  },
  logoutButton: {
    padding: 10,
  },
  statsContainer: {
    padding: 20,
  },
  sectionTitle: {
    fontSize: 18,
    fontWeight: 'bold',
    color: '#333',
    marginBottom: 15,
  },
  statsGrid: {
    gap: 15,
  },
  statCard: {
    backgroundColor: '#fff',
    borderRadius: 10,
    padding: 20,
    borderLeftWidth: 4,
    shadowColor: '#000',
    shadowOffset: {
      width: 0,
      height: 1,
    },
    shadowOpacity: 0.1,
    shadowRadius: 2,
    elevation: 2,
  },
  statCardContent: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
  },
  statCardLeft: {
    flex: 1,
  },
  statCardValue: {
    fontSize: 24,
    fontWeight: 'bold',
    color: '#333',
  },
  statCardTitle: {
    fontSize: 14,
    color: '#666',
    marginTop: 5,
  },
  statCardIcon: {
    width: 50,
    height: 50,
    borderRadius: 25,
    justifyContent: 'center',
    alignItems: 'center',
  },
  quickActionsContainer: {
    padding: 20,
    paddingTop: 0,
  },
  quickActionCard: {
    backgroundColor: '#fff',
    borderRadius: 10,
    padding: 15,
    marginBottom: 10,
    flexDirection: 'row',
    alignItems: 'center',
    shadowColor: '#000',
    shadowOffset: {
      width: 0,
      height: 1,
    },
    shadowOpacity: 0.1,
    shadowRadius: 2,
    elevation: 2,
  },
  quickActionIcon: {
    width: 40,
    height: 40,
    borderRadius: 20,
    backgroundColor: '#f0f0ff',
    justifyContent: 'center',
    alignItems: 'center',
    marginRight: 15,
  },
  quickActionContent: {
    flex: 1,
  },
  quickActionTitle: {
    fontSize: 16,
    fontWeight: '600',
    color: '#333',
  },
  quickActionDescription: {
    fontSize: 14,
    color: '#666',
    marginTop: 2,
  },
  userInfoContainer: {
    padding: 20,
    paddingTop: 0,
  },
  userInfoCard: {
    backgroundColor: '#fff',
    borderRadius: 10,
    padding: 20,
    shadowColor: '#000',
    shadowOffset: {
      width: 0,
      height: 1,
    },
    shadowOpacity: 0.1,
    shadowRadius: 2,
    elevation: 2,
  },
  userInfoRow: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    paddingVertical: 8,
    borderBottomWidth: 1,
    borderBottomColor: '#f0f0f0',
  },
  userInfoLabel: {
    fontSize: 14,
    color: '#666',
    fontWeight: '500',
  },
  userInfoValue: {
    fontSize: 14,
    color: '#333',
    fontWeight: '600',
  },
});

export default DashboardScreen;